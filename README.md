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

# Training AI with Unity ML-Agents: Hide and Seek Game

In this project, I've dived into the exciting world of Unity ML-Agents, aiming to teach AI-controlled characters to a hide and Seek game with AI agents that learn and adapt through reinforcement learning.

## Overview

This repository showcases the evolution of a Hide and Seek game in Unity. By leveraging Unity ML-Agents, I've taken this project to new heights, allowing AI agents to learn and employ sophisticated strategies and decision-making skills.

## Reinforcement Learning (RL) Method

### CustomAgent

At the heart of this project is the **CustomAgent** script. This is where the magic happens. Here's how it all comes together:

- **Initialization:** The agent is initialized, and references or necessary setups are performed.
- **Observations:** The agent collects observations about the game environment. This includes information about the agent's position and the hiding spot's position, among other things.
- **Actions:** The agent receives action commands and determines how to move within the game world. The AI decides whether to move left, right, forward, or stand still, and this decision is influenced by the reinforcement learning model.
- **Rewards:** Rewards are assigned based on the agent's actions. For example, a positive reward might be given for successfully reaching a hiding spot, while a negative reward may be assigned for hitting a wall.
- **Heuristic Control:** The script allows for manual control for heuristic-based testing, enabling us to guide the agent's behavior directly.

### HidingSpot

To add depth to the AI's gameplay strategy, I've included the **HidingSpot** script. This script represents the potential hiding spots for the Hider AI. The agent evaluates whether a hiding spot is available and leverages this information to enhance its game-winning strategy.

## Training Process

### Observation

The agent's journey begins with observations—it's how the agent perceives its environment. These observations include details like the agent's own position and the location of potential hiding spots.

### Action

The agent receives action commands that dictate how it moves within the game world. These actions are the outcome of the reinforcement learning model and profoundly affect the agent's movement.

### Reward

Rewards are assigned based on the agent's actions. Achieving a hiding spot might earn the agent a reward, while collisions with walls result in penalties.

### Learning

Throughout the learning process, the agent adjusts its actions to maximize rewards and minimize penalties. The reinforcement learning model is dynamic, adapting and evolving to transform the AI into a more skillful player.
