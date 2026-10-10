const AlertType = {
    INFO: "INFO",
    ERROR: "ERROR",
    SUCCESS: "SUCCESS",
    WARNING: "WARNING"
} as const;

export type AlertType = typeof AlertType[keyof typeof AlertType];

interface AlertProps {
    type: AlertType;
    title?: string;
    message: string;
}

const alertStyles: Record<AlertType, string> = {
    [AlertType.INFO]: "text-fg-brand-strong bg-brand-softer",
    [AlertType.ERROR]: "text-fg-danger-strong bg-danger-soft",
    [AlertType.SUCCESS]: "text-fg-success-strong bg-success-soft",
    [AlertType.WARNING]: "text-fg-warning bg-warning-soft",
};

export default function AlertBanner({type, title, message}: AlertProps) {
    const style = alertStyles[type];

    return (
        <div className={`p-4 mb-4 text-sm rounded-base ${style}`} role="alert">
            {title && <span className="font-medium">{title}: </span>}
            {message}
        </div>
    )
}