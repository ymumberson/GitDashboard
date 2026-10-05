import { CartesianGrid, Line, LineChart, ResponsiveContainer, Tooltip, XAxis } from "recharts";

interface CommitsOverTimeProps {
    commitsByDate: Record<string, number>;
}

export default function CommitsOverTime({commitsByDate}: CommitsOverTimeProps) {
   const data = Object.entries(commitsByDate)
        .map(([date, commits]) => ({
            date,
            commits
        }))
        .sort((a, b) => a.date.localeCompare(b.date));

        return (
            <div>
                <h2>Commits over time</h2>
                <ResponsiveContainer width="100%" height={300}>
                    <LineChart data={data}>
                        <CartesianGrid strokeDasharray="3 3" />
                        <XAxis dataKey="date" />
                        <Tooltip />
                        <Line 
                            type="monotone"
                            dataKey="commits"
                            stroke="#8884d8"
                            activeDot={{ r: 8 }}
                            animationDuration={1000}
                        />
                    </LineChart>
                </ResponsiveContainer>
            </div>
        )
}