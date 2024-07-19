# Peixinho Decoup
path to code: Peixinho_demo_v4/PeixinhoDecoup

Peixinho Decoup or GoFish is a card game developed using MonoGame. This project includes game logic, card loading and display, and basic user interaction.

## Overview

The goal of this project is to create a card game where players can interact with cards in a graphical interface. The game logic is separated from the view to facilitate maintenance and expansion of the code.

## Architecture

This project employs the Model-View-Controller (MVC) architecture, inspired by the principles outlined by Krasner and Pope. This design pattern effectively separates concerns within the application, facilitating modular development and maintenance:

- **Model**: Manages the core game logic and state, encapsulated within the `FishGame` module.
- **View**: Handles the graphical representation and rendering of the game, defined in the `PeixinhoDecoup` module.
- **Controller**: Processes user interactions and input, bridging the Model and View components to ensure cohesive operation.

## Project Structure

- **PeixinhoDecoup**: Contains the main game logic and view implementation.
- **FishGame**: Contains the game model and logic.
- **Content**: Contains all the content files including card images, fonts, and other assets.

## Getting Started

### Prerequisites

- MonoGame Framework
- Visual Studio or any other compatible C# IDE

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/your-username/peixinho-decoup.git
   ```
   
### Open the project in your IDE.

Restore the NuGet packages:

Adding Content Files
Add your content files (e.g., images, audio) to the Content directory.
Open Content.mgcb in the MGCB Editor.
Add the new content files using the editor:
Click the green plus icon to add an existing file.
Choose whether to copy the file to the project's content directory or add a link to it.

### Running the Game

Build the solution in your IDE.
Run the project.

