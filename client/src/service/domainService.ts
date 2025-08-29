const API_BASE_URL = import.meta.env.VITE_API_URL || "http://localhost:65453/api";

export interface DomainResult {
  domain: string;
  ip: string;
  hostedAt: string;
  updatedAt: string;
  ttl: number;
  nameServers?: string[];
  whoIs?: string;
}

export async function fetchDomain(domainName: string): Promise<DomainResult> {
  try {
    const response = await fetch(`${API_BASE_URL}/domain/${domainName}`);

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`Erro ${response.status}: ${errorText}`);
    }

    const data: DomainResult = await response.json();
    return data;

  } catch (error: any) {
    throw new Error(error.message);
  }
}
