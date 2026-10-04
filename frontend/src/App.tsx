import { useState } from 'react';
import './App.css'
import Dashboard from './Components/Dashboard';
import DashboardInput from './Components/DashboardInput';

function App() {
  const [owner, setOwner] = useState("");
  const [name, setName] = useState("");

  function handleRepositorySubmit(newOwner: string, newName: string) {
    setOwner(newOwner);
    setName(newName);
  }

  return (
    <div>
      <h1>GitDashboard</h1>
      <DashboardInput onSubmit={handleRepositorySubmit}/>
      <Dashboard owner={owner} name={name}/>
    </div>
  )
}

export default App
