import { useState } from "react";

interface DashboardInputProps {
    onSubmit: (owner: string, name: string) => void;
    isLoading: boolean;
}

export default function DashboardInput({onSubmit, isLoading}: DashboardInputProps) {
    const [owner, setOwner] = useState("ymumberson");
    const [name, setName] = useState("gitdashboard");
    const [validationError, setValidationError] = useState<string | null>(null);

    function handleSubmit(event: React.SubmitEvent<HTMLFormElement>) {
        event.preventDefault();

        const trimmedOwner = owner.trim();
        const trimmedName = name.trim();

        if (!trimmedOwner || !trimmedName) {
            setValidationError(
                "Please enter both a GitHub username and repository name."
            );
            return;
        }

        setValidationError(null);
        onSubmit(owner, name);
    }
    
    return (
        <form onSubmit={handleSubmit}>
            <div>
                <label htmlFor="owner">Username</label>
                <input 
                    id="owner"
                    value={owner}
                    onChange={event => setOwner(event.target.value)}
                />
            </div>

            <div>
                <label htmlFor="name">Repository</label>
                <input
                    id="name"
                    value={name}
                    onChange={event => setName(event.target.value)}
                />
            </div>

            {validationError && (
                <p role="alert">{validationError}</p>
            )}

            <button type="submit" disabled={isLoading}>
                Analyse
            </button>
        </form>
    );
}