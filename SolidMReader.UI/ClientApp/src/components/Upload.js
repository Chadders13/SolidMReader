import React, { Component } from 'react';

export class Upload extends Component {
    static displayName = Upload.name;

    constructor(props) {
        super(props);
        this.state = {
            selectedFile: null,
            uploadResult: null,
            error: null
        };
        this.handleFileChange = this.handleFileChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleFileChange(event) {
        this.setState({ selectedFile: event.target.files[0] });
    }

    async handleSubmit(event) {
        event.preventDefault();
        this.setState({ error: null, uploadResult: null });

        const token = localStorage.getItem('authToken');
        if (!token) {
            this.setState({ error: "You must be logged in to upload files." });
            return;
        }

        const formData = new FormData();
        formData.append('file', this.state.selectedFile);

        try {
            const response = await fetch('/meter-reading-uploads', {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}` 
                },
                body: formData
            });

            if (response.ok) {
                const result = await response.json();
                this.setState({ uploadResult: result });
            } else {
                this.setState({ error: "Error uploading file. Please try again." });
            }
        } catch (err) {
            this.setState({ error: "An unexpected error occurred." });
            console.error(err);
        }
    }

    render() {
        return (
            <div>
                <h2>Upload Meter Readings</h2>
                <form onSubmit={this.handleSubmit}>
                    <input type="file" accept=".csv" onChange={this.handleFileChange} />
                    <button type="submit" disabled={!this.state.selectedFile}>Upload</button>
                </form>

                {this.state.error && <p style={{ color: 'red' }}>{this.state.error}</p>}

                {this.state.uploadResult && (
                    <div>
                        <h3>Upload Results</h3>
                        <p>Successful Readings: {this.state.uploadResult.successful}</p>
                        <p>Failed Readings: {this.state.uploadResult.failed}</p>
                        {this.state.uploadResult.failedReadings.length > 0 && (
                            <div>
                                <h4>Failed Readings Details:</h4>
                                <ul>
                                    {this.state.uploadResult.failedReadings.map((reading, index) => (
                                        <li key={index}>{reading}</li>
                                    ))}
                                </ul>
                            </div>
                        )}
                    </div>
                )}
            </div>
        );
    }
}
