# 🌀 N‑Mass Points Pendulum Simulator

A **scientific and visual exploration** of multi‑pendulum dynamics, inspired by research on chaotic motion of coupled pendulums. This simulator models pendulums with **3 or more masses**, solves their motion using rigorous physics, and provides a **real‑time visualizer** with velocity‑based trails.

---

## 🔬 Research Basis

This project is grounded in scientific research and builds on key references:

- **D. Assêncio**, *“Double pendulum: Hamiltonian formulation”* ([Link](https://dassencio.org/33))  
  Derivation of double pendulum equations from Hamiltonian/Lagrangian mechanics.

- **J. Jiménez‑López & V.J. García‑Garrido**, *“Chaos and Regularity in the Double Pendulum with Lagrangian Descriptors”* ([arxiv](https://arxiv.org/html/2403.07000v1?utm_source=chatgpt.com))  
  Quantification of chaos using Lagrangian descriptors.

- **B. Yesilyurt**, *“Equations of Motion Formulation of a Pendulum Containing N-point Masses”* ([arxiv](https://arxiv.org/pdf/1910.12610?utm_source=chatgpt.com))  
  General formulation for **n‑mass pendulums**, providing the framework used for simulations with 3 or more masses.

Key points from these works implemented:

- Equations of motion derived from **Lagrangian / Hamiltonian mechanics**.  
- **Double pendulum (n = 2)**: classical coupled nonlinear ODEs.  
- **Multi-mass pendulums (n ≥ 3)**: generalized via mass matrix and coupling terms (see `PendulumSolver.cs`).  
- **Runge‑Kutta 4th order (RK4)** integration with configurable sub-steps for stability and accuracy.  
- Sensitivity to initial conditions and emergence of chaotic dynamics naturally appear in simulations.

---

## 🧮 Mathematics Behind It

<img width="767" height="224" alt="image" src="https://github.com/user-attachments/assets/4b5b7149-3663-4a8f-a8c1-9c7f6bc55ef5" />


- **Double pendulum (n = 2)**:  

- **General n‑mass pendulum (n ≥ 3)**:  


- **Integration**: RK4 with `subSteps` per frame:  

- **Chaos and energy**: Following Jiménez‑López & García‑Garrido, chaos fraction depends on energy, mass ratios, and length ratios.

---

## 🎮 Features

- ✅ Supports **3 or more pendulum masses**  
- ✅ Real-time visualization with **velocity-based trail colors**  
- ✅ Custom shader for smooth, colorful trails  
- ✅ Adjustable **rod width**, **mass radius**, **trail length**, and **velocity gradient**  
- ✅ Sensitive dynamics, allowing chaos and regularity studies  

---

## ⚙️ Usage

1. Open the project in **Unity**.  
2. Add `PendulumSolver` and `PendulumFullRenderer` to an empty GameObject.  
3. Configure mass points (`Mass`, `AttachedRodLength`, `InitialAngleDegrees`, `AngularVelocity`).  
4. Adjust `subSteps` in `PendulumSolver` for numerical stability.  
5. Customize visual parameters in `PendulumFullRenderer` (trail length, gradient, rod width, mass radius).  
6. Press **Play** to simulate and visualize pendulum motion.

---

## 🎨 Visualization

- Trails colored by instantaneous angular velocity.  
- Rods connecting masses with configurable colors and widths.  
- Mass points rendered as spheres, with color indicating speed.  
- Z-offsets per mass ensure distinct trails for multiple pendulums.

---

## 📚 References

- Assêncio, D., *Double pendulum: Hamiltonian formulation*, https://dassencio.org/33  
- Jiménez‑López, J. & García‑Garrido, V.J., *Chaos and Regularity in the Double Pendulum with Lagrangian Descriptors*, arXiv:2403.07000  
- Yesilyurt, B., *Equations of Motion Formulation of a Pendulum Containing N-point Masses*, arXiv:1910.12610  
