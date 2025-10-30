# 🌀 N-Mass Points Pendulum Simulator

A **scientific and visual exploration** of multi-pendulum dynamics, inspired by research on chaotic motion of coupled pendulums. This simulator models pendulums with **3 or more masses**, solves their motion using rigorous physics, and provides a **real-time visualizer** with velocity-based trails.

---

## 🔬 Research Basis

This project is grounded in scientific research, following methods outlined in:

**Double Pendulum Dynamics** – T. Müller et al., Max Planck Institute  
[PDF](https://www2.mpia-hd.mpg.de/homes/tmueller/pdfs/doublePendulum.pdf)

Key points from the paper implemented:

- Equations of motion derived from **Lagrangian mechanics**.
- Coupled, nonlinear system of ordinary differential equations (ODEs) for **n pendulum masses**.
- **Runge-Kutta 4th order (RK4)** integration for stable and accurate time evolution.
- Supports arbitrary number of masses (≥3), including **chaotic motion** analysis.

---

## 🧮 Mathematics Behind It

- **Single pendulum:**  
  \[
  \alpha = -\frac{g}{L} \sin(\theta)
  \]

- **Double pendulum:** Nonlinear coupled ODEs solved per Müller et al., with accelerations computed as:  
\[
\ddot{\theta}_i = f(\theta_1, \theta_2, \dots, \theta_n, \dot{\theta}_1, \dots, \dot{\theta}_n, m_i, L_i)
\]

- **Multiple pendulums (n > 2):**  
  Generalized mass matrix and coupling terms are solved with **linear algebra** and Gaussian elimination.

- **Integration:** **RK4 method** with configurable sub-steps for high-precision simulation.

---

## 🎮 Features

- ✅ Supports **3 or more pendulum masses**.
- ✅ Real-time visualization with **velocity-based trail colors**.
- ✅ Custom shader for smooth, colorful trails.
- ✅ Adjustable **rod width**, **mass radius**, and **trail length**.
- ✅ Chaos-friendly physics, allowing study of sensitive dependence on initial conditions.

---

## ⚙️ Usage

1. Open project in **Unity**.
2. Add `PendulumSolver` and `PendulumFullRenderer` components to an empty GameObject.
3. Configure **mass points** in the inspector (`Mass`, `Rod Length`, `Initial Angle`).
4. Press **Play** to simulate and visualize pendulum motion.

---

## 🎨 Visualization

- Trails are colored according to **mass velocity**, using a gradient.
- Rods and masses are drawn with vibrant, configurable colors.
- Supports multiple mass points simultaneously with smooth, connected trails.

---

## 📚 References

- Müller, T., et al., *Double Pendulum Dynamics*, Max Planck Institute  
  [PDF](https://www2.mpia-hd.mpg.de/homes/tmueller/pdfs/doublePendulum.pdf)
