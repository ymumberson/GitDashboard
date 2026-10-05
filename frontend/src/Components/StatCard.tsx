interface StatCardProps {
    title: string;
    value: number;
}

export default function StatCard({
    title,
    value
}: StatCardProps) {
    return (
        <div>
            <div>
                {title}
            </div>

            <div>
                {value.toLocaleString()}
            </div>
        </div>
    );
}