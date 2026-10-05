interface StatCardProps {
    title: string;
    value: number;
}

export default function StatCard({
    title,
    value
}: StatCardProps) {
    return (
        <div className="p-4 mb-4 text-sm text-fg-brand-strong rounded-base bg-brand-softer">
            <span className="font-medium">{title}</span> {value.toLocaleString()}
        </div>
    );
}