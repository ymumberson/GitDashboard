export interface RepositoryStats {
    totalCommits: number;
    commitsByAuthor: Record<string, number>;
    commitsByDate: Record<string, number>;
}