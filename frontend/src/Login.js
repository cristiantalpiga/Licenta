import React, {
    useState
}
from "react";

export default function Login() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");

	//const API_URL = process.env.REACT_APP_API_URL || "http://localhost:5000";
	const API_URL = "http://localhost:5000";

	const handleSubmit = async(e) => {
	e.preventDefault();
	setError("");

	try {
		const response = await fetch(`${API_URL}/api/Auth/login`, {
			method: "POST",
			headers: {
				"Content-Type": "application/json"
			},
			body: JSON.stringify({
				email,
				password
			})
		});

		if (response.ok) {
			const data = await response.json();
			localStorage.setItem("token", data.token);
			window.location.href = '/angajati';
			alert("Autentificare reușită!");
			// TODO: redirecționare sau navigare
		} else if (response.status === 401) {
			setError("Email sau parolă greșită.");
		} else {
			setError("Eroare neașteptată. Încearcă din nou.");
		}
	} catch (err) {
		console.error("Eroare conexiune:", err);
		setError(`Conexiunea la server a eșuat. API_URL = ${API_URL}`);
	}
};

return (
	 < div className = "login-container" >
		 < h2 > Autentificare <  / h2 >
		 < form onSubmit = {
		handleSubmit
	}
	 >
	 < input
	type = "email"
		placeholder = "Email (ex: ana.popescu@licenta.ro)"
		value = {
		email
	}
	onChange = {
		(e) => setEmail(e.target.value)
	}
	required
	/>
			<input
			  type="password"
			  placeholder="Parolă"
			  value={password}
			  onChange={(e) => setPassword(e.target.value)}
			  required
			/ >
	 < button type = "submit" > Login <  / button >
		 <  / form > {
		error &&  < p style = { {
				color: "red",
				marginTop: "1rem"
			}
		}
		 > {
			error
		}
		 <  / p >
	}
	 <  / div > );
}
