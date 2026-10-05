import type { RepositoryStats } from "../types/RepositoryStats";

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