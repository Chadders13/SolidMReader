import React, {Component} from 'react';
import {NavItem, NavLink} from "reactstrap";
import {Link} from "react-router-dom";
import { Upload } from "./Upload";

export class Home extends Component {
    static displayName = Home.name;

    constructor(props) {
        super(props);
        this.state = {
            accountId: '',
            currentCount: 0,
            lastReading: null,
            error: null
        };
        this.fetchLastReading = this.fetchLastReading.bind(this);
        this.handleAccountIdChange = this.handleAccountIdChange.bind(this);
    }

    handleAccountIdChange(event) {
        this.setState({accountId: event.target.value});
    }

    async fetchLastReading() {
        const token = localStorage.getItem('authToken');
        if (!token) {
            this.setState({error: "You must be logged in to fetch data."});
            return;
        }

        try {
            const response = await fetch(`/Account/${this.state.accountId}/lastReading`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });
            const data = await response.json();
            this.setState({lastReading: data, error: null});
        } catch (err) {
            this.setState({lastReading: null, error: "Error fetching last reading: " + err.message});
        }
    }

    render() {
        const isLoggedIn = !!localStorage.getItem('authToken');
        return (
            <div>
                {isLoggedIn && (
                    <div>
                        <h1>Last Meter Reading</h1>
                        <div>
                            <label htmlFor="accountId">Account ID:</label>
                            <input
                                type="text"
                                id="accountId"
                                value={this.state.accountId}
                                onChange={this.handleAccountIdChange}
                            />

                        </div>
                        <p aria-live="polite">Current count: <strong>{this.state.lastReading?.meterReadValue}</strong>
                        </p>
                        <button className="btn btn-primary" onClick={this.fetchLastReading} disabled={!this.state.accountId}>
                            Fetch
                        </button>
                        
                        <Upload />
                    </div>
                )}
                {!isLoggedIn && (
                    <div>
                        <p>You must be logged in to view this page.</p>
                        <NavItem>
                            <NavLink tag={Link} className="text-dark" to="/login">Login</NavLink>
                        </NavItem>
                    </div>
                )}
            </div>

        );
    }
}
