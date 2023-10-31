# Unity3D Hide and Seek AIs

Welcome to my Unity3D Hide and Seek Game repository! This project serves as the foundation for an exciting Hide and Seek game in Unity, featuring AI-controlled characters with hiding and seeking capabilities. I embarked on this journey to experiment with Unity AI and created this mini-game, or more like a demo.

## Overview

This repository contains the source code for an engaging Hide and Seek game. It includes AI-controlled characters with both hiding and seeking capabilities. The AI's behavior is divided into roles of Hider and Seeker, and it features movement, interaction with objects, and advanced game mechanics. This repository serves as a solid foundation for creating an exciting Hide and Seek gaming experience in Unity.

### GameManager Script

To enhance the gameplay and manage game logic effectively, I've integrated the **GameManager** script:

- **GameManager:** The GameManager script is a crucial part of the game. It controls the game flow, including game duration, role assignment, and the transition between hiding and seeking phases. With this script, you can customize game parameters such as game duration and hiding time.

### Key Components

- AI Controller: Handles AI behavior, including movement, detection, and decision-making.
- GameManager: Manages the game's flow and logic, controlling the duration and role assignments.

## Features

- **Hiding and Seeking:** The AI can take on two roles: Hider and Seeker. Customize AI behaviors for each role.
- **Game Management:** The GameManager script manages the game's flow and logic, allowing you to define the game's duration and phases.

### Repository Branches

- **Main Branch:** The main branch contains the Unity folder, which includes the basic game and pathfinding method.

- **Reinforcement Learning Branch:** The reinforcement learning (RL) branch contains all the files required for implementing the RL method using the ML-Agents package, including dependencies. This method allows the AI to learn and adapt its behavior over time, making it more intelligent and dynamic.

For the reinforcement learning method, please check the [RL Method.md](https://github.com/DournauxNathan/Hide-n-Seek/blob/main/Rl%20Method.md).
