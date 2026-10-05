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
        <div className="w-full max-w-sm bg-neutral-primary-soft p-6 border border-default rounded-base shadow-xs">
            <form onSubmit={handleSubmit}>
                <h5 className="text-xl font-semibold text-heading mb-6">Analyse a repository</h5>
                <div className="mb-4">
                    <label htmlFor="owner"
                        className="block mb-2.5 text-sm font-medium text-heading"
                    >
                        Username
                    </label>
                    <input 
                        id="owner"
                        value={owner}
                        onChange={event => setOwner(event.target.value)}
                        className="bg-neutral-secondary-medium border border-default-medium text-heading text-sm rounded-base focus:ring-brand focus:border-brand block w-full px-3 py-2.5 shadow-xs placeholder:text-body"
                    />
                </div>

                <div>
                    <label
                        htmlFor="name"
                        className="block mb-2.5 text-sm font-medium text-heading"
                    >
                        Repository
                    </label>
                    <input
                        id="name"
                        value={name}
                        onChange={event => setName(event.target.value)}
                        className="bg-neutral-secondary-medium border border-default-medium text-heading text-sm rounded-base focus:ring-brand focus:border-brand block w-full px-3 py-2.5 shadow-xs placeholder:text-body"
                    />
                </div>

                {validationError && (
                    <div className="p-4 mt-6 text-sm text-fg-danger-strong rounded-base bg-danger-soft" role="alert">
                        {validationError}
                    </div>
                )}

                <button
                    type="submit"
                    disabled={isLoading}
                    className="my-6 text-white bg-brand box-border border border-transparent hover:bg-brand-strong focus:ring-4 focus:ring-brand-medium shadow-xs font-medium leading-5 rounded-base text-sm px-4 py-2.5 focus:outline-none w-full mb-3"
                >
                    Analyse
                </button>
            </form>
        </div>
    );
}