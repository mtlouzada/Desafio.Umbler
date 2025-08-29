import { useState } from "react";
import "./App.scss";
import { fetchDomain } from "./service/domainService";
import type { DomainResult } from "./service/domainService";

export default function Home() {
  const [domain, setDomain] = useState<string>("");
  const [result, setResult] = useState<DomainResult | null>(null);
  const [error, setError] = useState<string | null>(null);

  const handleSearch = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setError(null);
    setResult(null);

    if (!domain.includes(".")) {
      setError("Digite um domínio válido. Ex: umbler.com");
      return;
    }

    try {
      const data: DomainResult = await fetchDomain(domain);
      setResult(data);
    } catch (err: any) {
      setError(err.message);
    }
  };

  return (
    <div className="container">
      <h1>Consulta de Domínio</h1>
      <form className="form" onSubmit={handleSearch}>
        <input
          type="text"
          value={domain}
          placeholder="Digite o domínio (ex: google.com)"
          onChange={(e) => setDomain(e.target.value)}
        />
        <button type="submit">Buscar</button>
      </form>

      {error && <p style={{ color: "red" }}>{error}</p>}

      {result && (
        <div className="result">
          <p><strong>Domínio:</strong> {result.domain}</p>
          <p><strong>IP:</strong> {result.ip}</p>
          <p><strong>Servidor:</strong> {result.hostedAt}</p>
          <p><strong>Última atualização:</strong> {new Date(result.updatedAt).toLocaleString()}</p>
          <p><strong>TTL:</strong> {result.ttl}</p>
          {result.nameServers && result.nameServers.length > 0 && (
            <p><strong>Name Servers:</strong> {result.nameServers.join(", ")}</p>
          )}
        </div>
      )}
    </div>
  );
}
