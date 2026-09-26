const form = document.querySelector("#repo-form");
const result = document.querySelector("#result");

form.addEventListener("submit", async (event) => {
    event.preventDefault();

    const response = await fetch("/api/hello");
    const data = await response.json();

    result.textContent = data.message;
});