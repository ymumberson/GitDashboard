import type { RepositoryStats } from "../types/RepositoryStats";

export async function getRepositoryStats(owner: string, name: string): Promise<RepositoryStats> {
    const response = await fetch(
        `/api/analyse?owner=${encodeURIComponent(owner)}&name=${encodeURIComponent(name)}`
    );

    if (!response.ok) {
        throw new Error(`Failed to analyse repository: ${response.status}`);
    }

    return response.json();
}