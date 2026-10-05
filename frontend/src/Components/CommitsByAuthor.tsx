import { Bar, BarChart, CartesianGrid, ResponsiveContainer, Tooltip, XAxis, YAxis } from "recharts";

interface CommitsByAuthorProps {
    commitsByAuthor: Record<string, number>;
}

export default function CommitsByAuthor({commitsByAuthor}: CommitsByAuthorProps) {
    const data = Object.entries(commitsByAuthor).map(([author, commits]) => ({
        author,
        commits
    })).sort((a, b) => b.commits - a.commits);

    return (
        <div>
            <h2>Commits by author</h2>

            <ResponsiveContainer width="100%" height={300}>
                <BarChart
                    data={data}
                    layout="vertical"
                    margin={{
                        top: 10,
                        right: 20,
                        left: 20,
                        bottom: 10
                    }}
                >
                    <CartesianGrid
                        strokeDasharray="3 3"
                        horizontal={false}
                    />

                    <XAxis
                        type="number"
                        allowDecimals={false}
                    />

                    <YAxis
                        type="category"
                        dataKey="author"
                        width={120}
                    />

                    <Tooltip />

                    <Bar
                        dataKey="commits"
                        radius={[0, 6, 6, 0]}
                        animationDuration={1000}
                    />
                </BarChart>
            </ResponsiveContainer>
        </div>
    );
}