import type { RepositoryStats } from "../types/RepositoryStats";
import AlertBanner from "./AlertBanner";
import CommitsByAuthor from "./CommitsByAuthor";
import CommitsOverTime from "./CommitsOverTime";

interface DashboardProps {
    stats: RepositoryStats;
}

export default function Dashboard({stats}: DashboardProps) {
    return (
        <div className="flex flex-col">
            <h2 className="mt-6 mb-2 text-3xl font-bold tracking-tight text-heading md:text-4xl">Overview</h2>
            <AlertBanner title="Total commits" type="INFO" message={stats.totalCommits.toString()}/>

            <h2 className="mt-6 mb-2 text-3xl font-bold tracking-tight text-heading md:text-4xl">Commits by author</h2>
            <CommitsByAuthor commitsByAuthor={stats.commitsByAuthor} />

            <h2 className="mt-6 mb-2 text-3xl font-bold tracking-tight text-heading md:text-4xl">Commits over time</h2>
            <CommitsOverTime commitsByDate={stats.commitsByDate}/>
        </div>
    )
}