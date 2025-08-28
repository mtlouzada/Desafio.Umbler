const API_BASE_URL = process.env.REACT_APP_API_URL || "http://localhost:65453/api";

export async function fetchDomain(domainName) {
  try {
    const response = await fetch(`${API_BASE_URL}/domain/${domainName}`);

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`Erro ${response.status}: ${errorText}`);
    }

    const data = await response.json();
    return data;

  } catch (error) {
    throw new Error(error.message);
  }
}
