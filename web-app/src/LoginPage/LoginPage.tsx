import type { FormEvent } from "react";
import { Link } from "react-router";
import styles from "./LoginPage.module.scss";

function LoginPage() {
    // TODO: add form validation.
    return (
        <main className="page">
            <section className={styles.loginSection}>
                <h1 className={styles.title}>Login</h1>

                <form className={styles.form} onSubmit={preventDefault}>
                    <label className={styles.label}>Email
                        <input type="text"/>
                    </label>

                    <label className={styles.label}>Password
                        <input type="password"/>
                    </label>

                    <button className={styles.loginButton}>Login</button>

                    <Link to="/login" className={styles.forgotPasswordLink}>Forgot password</Link>
                </form>
            </section>
        </main>
    );
}

function preventDefault(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
}

export default LoginPage;
