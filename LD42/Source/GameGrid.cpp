#include "GameGrid.h"

#include "renderFeedback.h"

static float margin_top = 43.0f;
static float margin_bot = 40.0f;
static float margin_left = 37.0f;
static float margin_right = 42.0f;

static sf::Texture drive_tex;

GameGrid::GameGrid()
{
	GameGrid(5, 5);
}



GameGrid::GameGrid(int w, int h)
{
	drive_tex.loadFromFile("drive.png");
	drive.setTexture(drive_tex);

	border_size = 1.0f;
	bgColor = { 0,0,0 };
	borderColor = { 100, 0 , 0 };


	numCells = { w, h };
	selected = sf::Vector2i{ -1, -1 };

	cellData.resize(w*h);
	for (auto& c : cellData) { c = GameCell(); }

	RebuildVerts();
}

void GameGrid::RebuildVerts()
{
	cell_size = sf::Vector2f{
		((drive_tex.getSize().x - (margin_left + margin_right)) / numCells.x) - border_size,
		((drive_tex.getSize().y - (margin_top + margin_bot)) / numCells.y) - border_size
	};

	auto bounds = InteractBounds();

	border_verts = sf::VertexArray(sf::Quads, (numCells.x + 1) * 4 + (numCells.y + 1) * 4);
	int i = 0;
	for (int x = 0; x <= numCells.x; x++) { // VERTICAL LINES
		float xPos = bounds.left + (x * (cell_size.x + border_size));

		border_verts[i++].position = { xPos - border_size * 0.5f, bounds.top - border_size * 0.5f };
		border_verts[i++].position = { xPos - border_size * 0.5f, bounds.top + bounds.height - border_size * 0.5f };
		border_verts[i++].position = { xPos + border_size * 0.5f, bounds.top + bounds.height - border_size * 0.5f };
		border_verts[i++].position = { xPos + border_size * 0.5f, bounds.top - border_size * 0.5f };
	}

	for (int y = 0; y <= numCells.y; y++) { // HORIZONAL LINES
		float yPos = bounds.top + (y * (cell_size.y + border_size));

		border_verts[i++].position = { bounds.left - border_size * 0.5f, yPos - border_size * 0.5f };
		border_verts[i++].position = { bounds.left + bounds.width + border_size * 0.5f, yPos - border_size * 0.5f };
		border_verts[i++].position = { bounds.left + bounds.width + border_size * 0.5f, yPos + border_size * 0.5f };
		border_verts[i++].position = { bounds.left - border_size * 0.5f, yPos + border_size * 0.5f };
	}

	cell_verts = sf::VertexArray(sf::Quads, 4 * cellData.size());
	i = 0;
	for (int y = 0; y < numCells.y; y++) {
		for (int x = 0; x < numCells.x; x++) {
			sf::Vector2f pos = GetCellCenter({ x,y });
			sf::Vector2f tl = pos - (cell_size * 0.5f);
			sf::Vector2f br = pos + (cell_size * 0.5f);
			cell_verts[i++].position = tl;
			cell_verts[i++].position = { br.x, tl.y };
			cell_verts[i++].position = br;
			cell_verts[i++].position = { tl.x, br.y };
		}
	}

	for (int i = 0; i < border_verts.getVertexCount(); i++) border_verts[i].color = borderColor;
	for (int i = 0; i < cell_verts.getVertexCount(); i++) cell_verts[i].color = bgColor;
}

sf::Rect<float> GameGrid::InteractBounds() const
{
	return sf::Rect<float>(
		margin_left,
		margin_top,
		drive_tex.getSize().x - margin_right - margin_left,
		drive_tex.getSize().y - margin_bot - margin_top);

}

sf::Vector2f GameGrid::GetCellTopLeft(sf::Vector2i pos) const
{
	auto bounds = InteractBounds();
	return sf::Vector2f{
		bounds.left + pos.x * (cell_size.x + border_size),
		bounds.top + pos.y * (cell_size.y + border_size)
	};
}

sf::Vector2f GameGrid::GetCellCenter(sf::Vector2i pos) const
{
	return GetCellTopLeft(pos) + (cell_size + sf::Vector2f{ border_size, border_size }) * 0.5f;
}

int GameGrid::Drop(GameRequest& req)
{
	if (g_renderFeedback.hovered != this)
		return -1;

	int placedCells = 0;

	const int offset = g_renderFeedback.hover_cell.y * numCells.x + g_renderFeedback.hover_cell.x;
	const int limit = (offset + req.numcells - req.cellsplaced);

	for (int i = offset; i < limit; i++) {
		if (i >= cellData.size())
			return placedCells;

		if (cellData[i].prog_id >= 0)
			return placedCells;

		cellData[i].prog_id = req.prog_id;
		cellData[i].color = req.color;
		cellData[i].offset = req.offset + req.cellsplaced++;
		placedCells++;
	}

	return placedCells;
}

void GameGrid::Update(LogicFeedback& logicfb)
{
	// RebuildVerts();

	for (GameCell& gc : cellData) {
		if (gc.prog_id < 0)
			continue;

		for (int pid : logicfb.explode_progid)
			if (pid == gc.prog_id) {
				gc.color = sf::Color{ 100,100,100 };
				goto end_of_cell_loop;				// -----------> GOTO
			}

		for (int pid : logicfb.wipe_progid)
			if (pid == gc.prog_id) {
				gc.prog_id = -1;
				gc.color = sf::Color::Transparent;
				goto end_of_cell_loop;				// -----------> GOTO
			}

	end_of_cell_loop:
		continue;
	}

	int i = 0;
	for (GameCell& gc : cellData) {
		sf::Color cc = gc.color;

		cell_verts[i++].color = cc;
		cell_verts[i++].color = cc;
		cell_verts[i++].color = cc;
		cell_verts[i++].color = cc;
	}
}

void GameGrid::draw(sf::RenderTarget& target, sf::RenderStates states) const
{
	/*
	{
		ImGui::DragFloat("Left", &margin_left);
		ImGui::DragFloat("Right", &margin_right);
		ImGui::DragFloat("Top", &margin_top);
		ImGui::DragFloat("Bot", &margin_bot);
	}*/

	states.transform *= getTransform();

	// Render Feedback
	sf::Vector2f localToObj = states.transform.getInverse().transformPoint(g_renderFeedback.cursorPos);
	auto bounds = InteractBounds();
	if (bounds.contains(localToObj)) {
		g_renderFeedback.hovered = this;

		g_renderFeedback.hover_cell = sf::Vector2i{
			std::max(0, std::min(numCells.x - 1, int((localToObj.x - bounds.left) / (cell_size.x + border_size)))),
			std::max(0, std::min(numCells.y - 1, int((localToObj.y - bounds.top) / (cell_size.y + border_size))))
		};

		g_renderFeedback.prog_hovered = cellData[g_renderFeedback.hover_cell.y * numCells.x + g_renderFeedback.hover_cell.x].prog_id;
	}

	// Draw
	target.draw(drive, states);
	target.draw(cell_verts, states);
	target.draw(border_verts, states);

	if (selected.x >= 0) {
		// Draw Hover Highlight
		sf::RectangleShape hover;
		hover.setSize(cell_size);
		hover.setPosition(GetCellTopLeft(selected));
		hover.setFillColor(sf::Color{ 255,255,255,100 });
		hover.setOutlineColor(sf::Color::White);

		target.draw(hover, states);
	}

	////////////////////////////////////////////////////////////////////////// HOVER
	sf::RectangleShape hover;
	hover.setSize(cell_size);
	hover.setFillColor(sf::Color::Transparent);
	hover.setOutlineColor(sf::Color::Blue);
	hover.setOutlineThickness(border_size);

	if (g_renderFeedback.hovered == this) { // Draw Hover Highlight
		hover.setPosition(GetCellTopLeft(g_renderFeedback.hover_cell));
		target.draw(hover, states);
	}

	char buffer[20];
	sf::Text num = sf::Text(" ", g_console_font, int(cell_size.y * 0.8f));
	num.setFillColor(sf::Color(0, 0, 0, 100));
	for (int i = 0; i < cellData.size(); ++i)
	{
		const GameCell& c = cellData[i];
		if (c.prog_id < 0)
			continue;

		sf::Vector2f cell_tl = GetCellTopLeft(sf::Vector2i{ i % numCells.x, i / numCells.x });

		sprintf_s(buffer, 20, "%2d", c.offset);
		num.setString(buffer);
		num.setPosition(cell_tl);
		target.draw(num, states);

		if (g_renderFeedback.hovered != this) {
			if (c.prog_id == g_renderFeedback.prog_hover_prev) {
				hover.setPosition(cell_tl);
				target.draw(hover, states);
			}
		}
	}
}