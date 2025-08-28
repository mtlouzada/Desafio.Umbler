import { useState } from "react";
import "./App.css";

export default function Home() {
  const [domain, setDomain] = useState("");
  const [result, setResult] = useState(null);
  const [error, setError] = useState(null);

  const handleSearch = async (e) => {
    e.preventDefault();
    setError(null);
    setResult(null);

    if (!domain.includes(".")) {
      setError("Digite um domínio válido. Ex: google.com");
      return;
    }

    try {
      const response = await fetch(`http://localhost:65453/api/domain/${domain}`);
      if (!response.ok) {
        throw new Error(`Erro ${response.status}: ${response.statusText}`);
      }
      const data = await response.json();
      setResult(data);
    } catch (err) {
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
        </div>
      )}
    </div>
  );
}
