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
        getRepositoryStats(owner, name)
            .then(setStats)
            .catch(error => {
                setError(error instanceof Error ? error.message : 'Unknown error')
            });
    }, [owner, name]);

    if (error) {
        return <p>Error: {error}</p>
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