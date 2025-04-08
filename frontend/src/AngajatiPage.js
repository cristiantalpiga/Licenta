import React, { useEffect, useState } from 'react';

function AngajatiPage() {
  const [angajati, setAngajati] = useState([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [token, setToken] = useState(localStorage.getItem('token'));

  useEffect(() => {
    if (!token) return;

    fetch('http://localhost:5000/api/Angajati', {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    })
      .then((res) => {
        if (!res.ok) {
          throw new Error('Eroare la obținerea angajaților');
        }
        return res.json();
      })
      .then((data) => setAngajati(data))
      .catch((err) => console.error(err));
  }, [token]);

  const filtered = angajati.filter((a) =>
    `${a.nume} ${a.prenume}`.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="p-4">
      <h1 className="text-2xl font-bold mb-4">Lista Angajați</h1>

      <input
        type="text"
        placeholder="Caută angajat..."
        value={searchTerm}
        onChange={(e) => setSearchTerm(e.target.value)}
        className="border p-2 rounded-md w-full max-w-md mb-4"
      />

      <ul className="space-y-2">
        {filtered.map((a) => (
          <li key={a.id} className="border p-3 rounded-md">
            <div className="font-semibold">
              {a.prenume} {a.nume}
            </div>
            <div className="text-sm text-gray-600">{a.email}</div>
            <div className="text-sm text-gray-600">{a.telefon}</div>
            <div className="text-sm text-gray-600">Pozitie ID: {a.idPozitie}</div>
            <div className="text-sm text-gray-600">Departament ID: {a.idDepartament}</div>
          </li>
        ))}
      </ul>
    </div>
  );
}

export default AngajatiPage;
