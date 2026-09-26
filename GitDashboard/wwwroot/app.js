const form = document.querySelector("#repo-form");
const result = document.querySelector("#result");

form.addEventListener("submit", async (event) => {
    event.preventDefault();

    const path = document.querySelector("#repo-path").value;

    result.textContent = "Analysing...";

    try {
        const response = await fetch(
            `/api/analyse?path=${encodeURIComponent(path)}`
        );

        if (!response.ok) {
            throw new Error("Failed to analyse repository");
        }

        const commits = await response.json();

        result.innerHTML = `
            <h2>${commits.length} commits</h2>
            <ul>
                ${commits.slice(0, 10).map(commit => `
                    <li>
                        ${commit.author} — ${commit.date}
                    </li>
                `).join("")}
            </ul>
        `;
    } catch (error) {
        result.textContent = error.message;
    }
});