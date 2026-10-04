import { useState } from "react";

interface DashboardInputProps {
    onSubmit: (owner: string, name: string) => void;
}

export default function DashboardInput({onSubmit}: DashboardInputProps) {
    const [owner, setOwner] = useState("");
    const [name, setName] = useState("");

    function handleSubmit(event: React.SubmitEvent<HTMLFormElement>) {
        event.preventDefault();

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

            <button type="submit">
                Analyse
            </button>
        </form>
    );
}