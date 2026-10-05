import type { RepositoryStats } from "../types/RepositoryStats";
import CommitsOverTime from "./CommitsOverTime";

interface DashboardProps {
    stats: RepositoryStats;
}

export default function Dashboard({stats}: DashboardProps) {
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

            <CommitsOverTime commitsByDate={stats.commitsByDate}/>
        </div>
    )
}