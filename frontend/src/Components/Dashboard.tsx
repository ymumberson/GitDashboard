import type { RepositoryStats } from "../types/RepositoryStats";
import CommitsByAuthor from "./CommitsByAuthor";
import CommitsOverTime from "./CommitsOverTime";
import StatCard from "./StatCard";

interface DashboardProps {
    stats: RepositoryStats;
}

export default function Dashboard({stats}: DashboardProps) {
    return (
        <div>
            <h2>Overview</h2>
            <StatCard title="Total commits" value={stats.totalCommits}/>

            <CommitsByAuthor commitsByAuthor={stats.commitsByAuthor} />

            <CommitsOverTime commitsByDate={stats.commitsByDate}/>
        </div>
    )
}