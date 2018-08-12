#pragma once

#include <SFML/Graphics.hpp>
#include <SFML/Graphics/Rect.hpp>
#include <SFML/Graphics/RenderWindow.hpp>
#include <SFML/System/Err.hpp>
#include <SFML/System/Clock.hpp>
#include <SFML/Window/Event.hpp>

struct RenderFeedback {
	int prog_hovered = -1;
	int prog_hover_prev = -1;
	sf::Vector3f execPos;
	sf::Vector2f cursorPos;
	sf::Vector2f dragStartPos;
	sf::Vector2i hover_cell;
	sf::Vector2i drag_cells;
	sf::Time drag_time;
	const sf::Drawable* hovered;
	const sf::Drawable * dragged = false;
};

struct LogicFeedback {
	std::vector<int> wipe_progid;
	std::vector<int> placed_progid;
	std::vector<int> explode_progid;
};

extern RenderFeedback g_renderFeedback;
extern sf::Font g_console_font;
