import { Link } from "react-router-dom";
import styles from "./Home.module.css";

export default function Home() {
  return (
    <div className={styles.hero}>
      <h1 className={styles.title}>
        March Madness
        <span className={styles.year}>2025–26</span>
      </h1>
      <p className={styles.subtitle}>
        NCAA Men's Basketball Bracket Simulator
      </p>
      <div className={styles.actions}>
        <Link to="/bracket" className={styles.btnPrimary}>
          View Bracket
        </Link>
        <Link to="/simulate" className={styles.btnSecondary}>
          Run Simulation
        </Link>
      </div>
    </div>
  );
}
