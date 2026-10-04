import { useEffect, useState } from "react";
import type { RepositoryStats } from "../types/RepositoryStats";
import { getRepositoryStats } from "../api/repositoryApi";

interface DashboardProps {
    owner: string;
    name: string;
}

export default function Dashboard({owner, name}: DashboardProps) {
    const [stats, setStats] = useState<RepositoryStats | null>(null);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        setStats(null);
        setError(null);

        if (isNullOrWhiteSpace(owner) || isNullOrWhiteSpace(name))
            return;
        
        getRepositoryStats(owner, name)
            .then(setStats)
            .catch(error => {
                setError(error instanceof Error ? error.message : 'Unknown error')
            });
    }, [owner, name]);

    const isNullOrWhiteSpace = (value: string | null | undefined): boolean => value == null || value.trim() === "";

    if (error) {
        return <p>Error: {error}</p>
    }

    if (isNullOrWhiteSpace(owner) || isNullOrWhiteSpace(name)) {
        return <p>Please enter a username and repository</p>
    }

    if (!stats) {
        return <p>Loading...</p>
    }

    return (
        <div>
            <h2>Overview</h2>
            <p>Total commits: {stats.totalCommits}</p>

            <h2>Commits by author</h2>
            <ul>
                {
                    Object.entries(stats.commitsByAuthor).map(([author, count]) => (
                        <li key={author}>
                            {author}: {count}
                        </li>
                    ))
                }
            </ul>

            <h2>Commits by date</h2>
            <ul>
                {
                    Object.entries(stats.commitsByDate).map(([date, count]) => (
                        <li key={date}>
                            {date}: {count}
                        </li>
                    ))
                }
            </ul>
        </div>
    )
}