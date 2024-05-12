import React, { Component } from 'react';

export class Home extends Component {
  static displayName = Home.name;

  constructor(props) {
    super(props);
    this.state = {
      accountId: '', // Added state for account ID
      currentCount: 0,
      lastReading: null,
      error: null
    };
    this.incrementCounter = this.incrementCounter.bind(this);
    this.fetchLastReading = this.fetchLastReading.bind(this); // Bind fetch method
    this.handleAccountIdChange = this.handleAccountIdChange.bind(this); // Bind input change handler
  }

  incrementCounter() {
    this.setState({
      currentCount: this.state.currentCount + 1
    });
  }

  handleAccountIdChange(event) {
    this.setState({ accountId: event.target.value });
  }

  async fetchLastReading() {
    try {
      const response = await fetch(`/Account/${this.state.accountId}/lastReading`);
      const data = await response.json();
      this.setState({ lastReading: data, error: null });
    } catch (err) {
      this.setState({ lastReading: null, error: "Error fetching last reading: " + err.message });
    }
  }

  render() {
    return (
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
            <button onClick={this.fetchLastReading} disabled={!this.state.accountId}>
              Fetch
            </button>
          </div>
          {/* ... (conditional display of lastReading or error) ... */}
          <p aria-live="polite">Current count: <strong>{this.state.lastReading?.meterReadValue}</strong> </p>
          <button className="btn btn-primary" onClick={this.incrementCounter}>Increment</button>
        </div>
    );
  }
}


// import React, { Component } from 'react';
//
// export class Home extends Component {
//   static displayName = Home.name;
//    
//     static accountId = 2244;
//     static lastReading = "00000";
//     static error = "";
//
//     fetchLastReading = async () => {
//     try {
//       const response = await fetch(`/api/meterreadings/Account/${accountId}/lastReading`);
//       const data = await response.json();
//       setLastReading(data);
//       setError(null);
//     } catch (err) {
//       setError("Error fetching last reading: " + err.message);
//       setLastReading(null); // Clear previous data on error
//     }
//   };
//    
//   render() {
//     return (
//         <div>
//           <h1>Last Meter Reading</h1>
//           <div>
//             <label htmlFor="accountId">Account ID:</label>
//             <input
//                 type="text"
//                 id="accountId"
//                 value={accountId}
//                 onChange={(e) => setAccountId(e.target.value)}
//             />
//             <button onClick={fetchLastReading} disabled={!accountId}>
//               Fetch
//             </button>
//           </div>
//
//           {error ? (
//               <p style={{color: 'red'}}>{error}</p> // Display error message
//           ) : lastReading ? (
//               <div>
//                 <p>Account ID: {lastReading.accountId}</p>
//                 <p>Meter Reading Date Time: {lastReading.meterReadingDateTime}</p>
//                 <p>Meter Read Value: {lastReading.meterReadValue}</p>
//               </div>
//           ) : null}
//         </div>
//     );
//   }
// }
//
// // import React, {useState, useEffect} from 'react';
// //
// // function Home() {
// //   const [accountId, setAccountId] = useState('');
// //   const [lastReading, setLastReading] = useState(null);
// //   const [error, setError] = useState(null);
// //
// //   const fetchLastReading = async () => {
// //     try {
// //       const response = await fetch(`/api/meterreadings/Account/${accountId}/lastReading`);
// //       const data = await response.json();
// //       setLastReading(data);
// //       setError(null);
// //     } catch (err) {
// //       setError("Error fetching last reading: " + err.message);
// //       setLastReading(null); // Clear previous data on error
// //     }
// //   };
// //
// //   return (
// //       <div>
// //         <h1>Last Meter Reading</h1>
// //         <div>
// //           <label htmlFor="accountId">Account ID:</label>
// //           <input
// //               type="text"
// //               id="accountId"
// //               value={accountId}
// //               onChange={(e) => setAccountId(e.target.value)}
// //           />
// //           <button onClick={fetchLastReading} disabled={!accountId}>
// //             Fetch
// //           </button>
// //         </div>
// //
// //         {error ? (
// //             <p style={{ color: 'red' }}>{error}</p> // Display error message
// //         ) : lastReading ? (
// //             <div>
// //               <p>Account ID: {lastReading.accountId}</p>
// //               <p>Meter Reading Date Time: {lastReading.meterReadingDateTime}</p>
// //               <p>Meter Read Value: {lastReading.meterReadValue}</p>
// //             </div>
// //         ) : null}
// //       </div>
// //   );
// // }
// //
// // export default Home;
