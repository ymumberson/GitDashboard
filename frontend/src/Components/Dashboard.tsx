import type { RepositoryStats } from "../types/RepositoryStats";
import CommitsByAuthor from "./CommitsByAuthor";
import CommitsOverTime from "./CommitsOverTime";

interface DashboardProps {
    stats: RepositoryStats;
}

export default function Dashboard({stats}: DashboardProps) {
    return (
        <div>
            <h2>Overview</h2>
            <p>Total commits: {stats.totalCommits}</p>

            <CommitsByAuthor commitsByAuthor={stats.commitsByAuthor} />

            <CommitsOverTime commitsByDate={stats.commitsByDate}/>
        </div>
    )
}