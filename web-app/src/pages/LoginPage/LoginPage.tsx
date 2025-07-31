import { useState, type FormEvent, type MouseEvent, type Dispatch, type SetStateAction } from "react";
import { Link } from "react-router";
import styles from "./LoginPage.module.scss";
import Fetcher from "../../scripts/Fetcher";
import preventDefault from "../../scripts/FormPreventDefault";

type FieldStatus = {
    emailIsValid: boolean | null;
    passwordIsValid: boolean | null;
};

type LoginStatus = "loading" | "success" | "failed" | null | "error";

export default function LoginPage() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [fieldStatus, setFieldStatus] = useState<FieldStatus>({ emailIsValid: null, passwordIsValid: null });
    const [loginStatus, setLoginStatus] = useState<LoginStatus>(null);

    const handleEmail = (e: FormEvent<HTMLInputElement>) => setEmail(e.currentTarget.value);
    const handlePassword = (e: FormEvent<HTMLInputElement>) => setPassword(e.currentTarget.value);

    const loginClick = (e: MouseEvent<HTMLButtonElement>) => {
        const button = e.currentTarget;
        pressLoginButton(button);

        const isValid = validate(email, password, setFieldStatus);
        if (isValid)
            login(email, password, setLoginStatus);
    };

    const emailClass = `${styles.errorText} ${fieldStatus.emailIsValid === false && styles.errorTextShow}`;
    const passwordClass = `${styles.errorText} ${fieldStatus.passwordIsValid === false && styles.errorTextShow}`;
    const loginTextClass = getLoginTextClass(loginStatus);
    const loginText = getLoginText(loginStatus);

    return (
        <main className={`page ${styles.page}`}>
            <section className={styles.loginSection}>
                <h1 className={styles.title}>Login</h1>

                <form className={styles.form} onSubmit={preventDefault}>
                    <div className={styles.inputGroup}>
                        <input type="text" placeholder="E-mail" onChange={handleEmail}/>
                        <p className={emailClass}>Enter your e-mail address</p>
                    </div>

                    <div className={styles.inputGroup}>
                        <input type="password" placeholder="Password" onChange={handlePassword}/>
                        <p className={passwordClass}>Enter your password</p>
                    </div>

                    <p className={loginTextClass}>{ loginText }</p>

                    <div className={styles.buttons}>
                        <button className={styles.loginButton} onClick={loginClick}>Login</button>
                        <button className={styles.registerButton}>Register</button>
                    </div>

                    <Link to="/login" className={styles.forgotPasswordLink}>Forgot password?</Link>
                </form>
            </section>
        </main>
    );
}

/**
 * Simulates a button press effect (like the CSS :active state) for keyboard users.
 * 
 * This function temporarily adds a CSS class to the given button element to visually
 * mimic a press interaction when activated via the keyboard (e.g., Enter key). It reads
 * the transition duration from the button's `--transition-duration` CSS variable to ensure
 * the timing matches the CSS animation.
 *
 * @param button - The HTML button element to apply the visual press effect to.
 */
function pressLoginButton(button: HTMLButtonElement) {
    button.classList.add(styles.loginButtonActive);

    const style = getComputedStyle(button);
    const timeStr = style.getPropertyValue("--transition-duration").trim();
    const time = timeStr.endsWith("ms")
        ? parseFloat(timeStr)
        : parseFloat(timeStr) * 1000;
    setTimeout(() => button.classList.remove(styles.loginButtonActive), time);
}

/**
 * Validates the given email and password by checking that they are non-empty.
 * Updates the corresponding field validity status.
 *
 * @param email - The email string to validate.
 * @param password - The password string to validate.
 * @param setFieldStatus - A state setter function that updates the validity of each field.
 * 
 * @returns `true` if both the email and password are non-empty; otherwise, `false`.
 */
function validate(email: string, password: string, setFieldStatus: Dispatch<SetStateAction<FieldStatus>>): boolean {
    const isEmailValid = email.length !== 0;
    const isPasswordValid = password.length !== 0;

    setFieldStatus(_old => ({ emailIsValid: isEmailValid, passwordIsValid: isPasswordValid }));

    return isEmailValid && isPasswordValid;
}

async function login(email: string, password: string, setLoginStatus: Dispatch<SetStateAction<LoginStatus>>) {
    setLoginStatus("loading");

    await Fetcher
        .post("api/v1/auth/login", { "content-type": "application/json" })
        .setParseResponse(async response => await response.json())
        .setOnOk(_ => setLoginStatus("success"))
        .setOnNonOk(_ => setLoginStatus("failed"))
        .setOnError(_ => setLoginStatus("error"))
        .fetch({ email, password });
}

function getLoginTextClass(loginStatus: LoginStatus): string {
    let classNames = styles.loginText;
    classNames += loginStatus === "success" ? ` ${styles.loginTextSuccess}` : "";
    classNames += loginStatus === "loading" ? ` ${styles.loginTextLoading}` : "";
    classNames += loginStatus === "failed" || loginStatus === "error" ? ` ${styles.loginTextFailed}` : "";

    return classNames;
}

function getLoginText(loginStatus: LoginStatus): string {
    switch(true) {
        case loginStatus === "loading":
            return "Loading...";

        case loginStatus === "success":
            return "Successfully logged in!";

        case loginStatus === "failed":
            return "Incorrect credentials.";

        case loginStatus === "error":
            return "Could not reach the server.";

        case loginStatus === null:
        default:
            return "";
    }
}
