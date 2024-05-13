import React, { Component } from 'react';

export class Login extends Component {
    static displayName = Login.name;

    constructor(props) {
        super(props);
        this.state = {
            username: '',
            password: '',
            error: null
        };
        this.handleUsernameChange = this.handleUsernameChange.bind(this);
        this.handlePasswordChange = this.handlePasswordChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleUsernameChange(event) {
        this.setState({ username: event.target.value });
    }

    handlePasswordChange(event) {
        this.setState({ password: event.target.value });
    }

    async handleSubmit(event) {
        event.preventDefault();
        this.setState({ error: null });

        try {
            const response = await fetch('/api/authenticate', {
                method: 'POST',
                headers: {'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    username: this.state.username,
                    password: this.state.password
                })
            });

            if (response.ok) {
                const data = await response.json();
                const token = data.token;
                localStorage.setItem('authToken', token);

                window.location.href = '/';
            } else {
                this.setState({ error: "Invalid username or password." });
            }
        } catch (err) {
            this.setState({ error: "Error during login: " + err.message });
        }
    }

    render() {
        return (
            <div>
                <h2>Login</h2>
                {this.state.error && <p style={{ color: 'red' }}>{this.state.error}</p>}
                <form onSubmit={this.handleSubmit}>
                    <div>
                        <label htmlFor="username">Username:</label>
                        <input
                            type="text"
                            id="username"
                            value={this.state.username}
                            onChange={this.handleUsernameChange}
                        />
                    </div>
                    <div>
                        <label htmlFor="password">Password:</label>
                        <input
                            type="password"
                            id="password"
                            value={this.state.password}
                            onChange={this.handlePasswordChange}
                        />
                    </div>
                    <button type="submit">Login</button>
                </form>
            </div>
        );
    }
}
