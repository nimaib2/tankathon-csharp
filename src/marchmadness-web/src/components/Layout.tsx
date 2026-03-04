import { NavLink, Outlet } from "react-router-dom";
import styles from "./Layout.module.css";

export default function Layout() {
  return (
    <div className={styles.layout}>
      <nav className={styles.nav}>
        <NavLink to="/" className={styles.brand}>
          March Madness
        </NavLink>
        <div className={styles.links}>
          <NavLink
            to="/bracket"
            className={({ isActive }) =>
              isActive ? `${styles.link} ${styles.active}` : styles.link
            }
          >
            Bracket
          </NavLink>
          <NavLink
            to="/simulate"
            className={({ isActive }) =>
              isActive ? `${styles.link} ${styles.active}` : styles.link
            }
          >
            Simulate
          </NavLink>
        </div>
      </nav>
      <main className={styles.main}>
        <Outlet />
      </main>
    </div>
  );
}
