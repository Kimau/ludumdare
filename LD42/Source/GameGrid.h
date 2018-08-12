#pragma once

#include "../imgui/imgui.h"
#include "imgui-sfml.h"

#include <SFML/Graphics.hpp>
#include <SFML/Graphics/Rect.hpp>
#include <SFML/Graphics/RenderWindow.hpp>
#include <SFML/System/Err.hpp>
#include <SFML/System/Clock.hpp>
#include <SFML/Window/Event.hpp>

#include <iostream>
#include <fstream>
#include <string>

#include "GameCell.h"
#include "renderFeedback.h"

class GameGrid : public sf::Drawable, public sf::Transformable {
public:
	GameGrid();
	GameGrid(int w, int h);

	void RebuildVerts();

	sf::Rect<float> InteractBounds() const;

	sf::Vector2f GetCellTopLeft(sf::Vector2i pos) const;
	sf::Vector2f GetCellCenter(sf::Vector2i pos) const;

	int Drop(GameRequest& req);
	void Update(LogicFeedback& logicfb);

	std::vector<GameCell> cellData;
	sf::Vector2i numCells;
	sf::Vector2f cell_size;
	float border_size;
	sf::Color bgColor;
	sf::Color borderColor;

	sf::Vector2i selected;


	sf::VertexArray border_verts;
	sf::VertexArray cell_verts;
	sf::Sprite drive;

private:
	virtual void draw(sf::RenderTarget& target, sf::RenderStates states) const;
};