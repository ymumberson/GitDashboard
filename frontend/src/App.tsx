import { useState } from 'react';
import './App.css'
import Dashboard from './Components/Dashboard';
import DashboardInput from './Components/DashboardInput';
import type { RepositoryStats } from './types/RepositoryStats';
import { getRepositoryStats } from './api/repositoryApi';
import GraphPlaceholder from './Components/GraphPlaceholder';
import LoadingSpinner from './Components/LoadingSpinner';

function App() {
  const [stats, setStats] = useState<RepositoryStats | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  function handleRepositorySubmit(owner: string, name: string) {
    setIsLoading(true);
    setError(null);
    setStats(null);

    if (isNullOrWhiteSpace(owner) || isNullOrWhiteSpace(name))
        return;
    
    getRepositoryStats(owner, name)
        .then(setStats)
        .catch(error => {
            setError(error instanceof Error ? error.message : 'Unknown error')
        })
        .finally(() => {
            setIsLoading(false);
        });
  }

  const isNullOrWhiteSpace = (value: string | null | undefined): boolean => value == null || value.trim() === "";

  return (
    <div className='px-12'>
      <div className="mb-10 mt-10 w-full text-center">
        <h1 className="text-5xl font-bold text-heading">Git Dashboard</h1>
      </div>
      <div className='flex flex-col w-full items-center'>
        <DashboardInput onSubmit={handleRepositorySubmit} isLoading={isLoading}/>
        {isLoading &&
          <div className="pt-6">
            <LoadingSpinner />
          </div>}
        {error && <p role="alert">{error}</p>}
        {stats && <Dashboard stats={stats}/>}
      </div>
    </div>
  )
}

export default App
