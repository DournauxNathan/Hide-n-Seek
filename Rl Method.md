# Training AI with Unity ML-Agents: Hide and Seek Game

Welcome to my Unity3D Hide and Seek Game repository, where I've integrated the powerful Unity ML-Agents framework to train AI-controlled characters for a more intelligent and dynamic gaming experience. In this project, I'll walk you through the process of teaching AI to move strategically to a hiding spot using reinforcement learning.

## Overview

This repository showcases my journey in enhancing a Hide and Seek game in Unity with AI agents that learn and adapt their behavior over time. With the integration of Unity ML-Agents, I've taken the game to the next level by training the AI agents to exhibit more sophisticated strategies and decision-making capabilities.

## Reinforcement Learning (RL) Method

### CustomAgent

The heart of this project lies in the **CustomAgent** script. This script is responsible for implementing the AI agent using ML-Agents for reinforcement learning. Here's an overview of how it works:

- **Initialization:** The agent is initialized, and references or necessary setups are performed.
- **Observations:** The agent collects observations about the game environment. This includes information about the agent's position and the hiding spot's position, among other things.
- **Actions:** The agent receives action commands and determines how to move within the game world. The AI decides whether to move left, right, forward, or stand still, and this decision is influenced by the reinforcement learning model.
- **Rewards:** Rewards are assigned based on the agent's actions. For example, a positive reward might be given for successfully reaching a hiding spot, while a negative reward may be assigned for hitting a wall.
- **Heuristic Control:** The script allows for manual control for heuristic-based testing, enabling us to guide the agent's behavior directly.

### HidingSpot

To improve the AI's gameplay strategy, we have also included the **HidingSpot** script. This script represents potential hiding spots for the Hider AI. The agent assesses whether a hiding spot is available and uses it to improve its chances of winning the game.

## Training Process

### Observation

The agent collects observations, which serve as its "perception" of the game world. This includes data such as its own position and the position of potential hiding spots.

### Action

The agent receives action commands, such as how to move within the game world. These actions are determined by the reinforcement learning model, and they impact the agent's movement.

### Reward

Rewards are assigned to the agent based on its actions. For instance, successfully reaching a hiding spot is rewarded, while running into a wall may incur a penalty.

### Learning

Through a process of trial and error, the agent learns how to optimize its actions to maximize rewards and minimize penalties. The reinforcement learning model adapts its behavior over time to become a more proficient player.

## Usage

1. Clone or download this repository.
2. Open it in Unity.
3. Use Unity's ML-Agents tools to train the AI agents.
4. Customize the AI behaviors and train the agents to improve their performance.
5. Enjoy playing the game with AI-controlled characters that have learned to move strategically to hiding spots.

This project represents an exciting exploration into the capabilities of Unity ML-Agents for training AI. The AI agents become more intelligent and dynamic as they learn and adapt their behavior, making the Hide and Seek game a thrilling and challenging experience.

Feel free to experiment with the AI behaviors, training settings, and other parameters to create the ultimate Hide and Seek experience using Unity and ML-Agents. Happy gaming!
