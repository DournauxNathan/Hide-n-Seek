# Training AI with Unity ML-Agents: Hide and Seek Game

Welcome to my Unity3D Hide and Seek Game repository. In this project, I've dived into the exciting world of Unity ML-Agents, aiming to teach AI-controlled characters to exhibit intelligent and dynamic behavior. Join me as I share my journey of enhancing a Hide and Seek game with AI agents that learn and adapt through reinforcement learning.

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

## Usage

1. Clone or download this repository.
2. Open it in Unity.
3. Dive into Unity's ML-Agents tools to train the AI agents.
4. Customize AI behaviors and tweak training settings to watch your agents flourish.
5. Immerse yourself in gameplay with AI-controlled characters that have become experts at strategic movement to hiding spots.

This project represents an exhilarating journey into the world of Unity ML-Agents, where AI agents evolve into intelligent, adaptable players. They learn and grow, adding depth and challenge to the Hide and Seek game.

Feel free to experiment with AI behaviors, training settings, and various parameters to create the ultimate Hide and Seek experience using Unity and ML-Agents. Join me in the world of AI-powered gaming, and let's embark on this thrilling adventure together!
