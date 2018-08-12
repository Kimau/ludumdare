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
#include <list>
#include <queue>
#include <chrono>

#include "renderFeedback.h"
#include "GameGrid.h"
#include "GameRequestBoard.h"

RenderFeedback g_renderFeedback;
sf::Font g_console_font;

std::mt19937 game_rand;
GameGrid ggrid;
GameRequestBoard grequest;

struct {
	bool visible;
	sf::Sprite spr;
	sf::Vector3f pos;
	sf::Vector3f dest;

	bool AtDest() const {
		return (abs(pos.x - dest.x) + abs(pos.y - dest.y) + abs(pos.z - dest.z)) < 1.0f;
	}

} garrow;

int running_pid = -1;
int prog_id_counter = 1;
float seconds_till_prog_Spawn = 0;

std::list<TermLine> termlines;
std::vector<GameProgram> progs;
std::queue<ProgInstructions> prog_todo;
struct {
	ProgInstructions todo;
	sf::Time time;
	int a;
	int b;
	int stage = 0;
} prog_doing;

sf::Color presetColours[] = {
	sf::Color{ 255, 0, 128 },
	sf::Color{ 0, 64, 128 },
	sf::Color{ 255, 128, 128 },
	sf::Color{ 128, 64, 0 },
	sf::Color{ 0, 255, 0 },
	sf::Color{ 64, 64, 128 },
	sf::Color{ 0, 128, 255 },
	sf::Color{ 64, 128, 0 },
	sf::Color{ 255, 0, 0 },
	sf::Color{ 64, 0, 128 },
	sf::Color{ 128, 128, 255 },
	sf::Color{ 128, 255, 0 },
	sf::Color{ 0, 0, 128 },
	sf::Color{ 0, 255, 128 },
	sf::Color{ 0, 128, 64 },
	sf::Color{ 0, 0, 255 },
	sf::Color{ 128, 64, 64 },
	sf::Color{ 128, 0, 255 },
	sf::Color{ 128, 0, 0 },
	sf::Color{ 0, 128, 0 },
	sf::Color{ 128, 255, 128 },
	sf::Color{ 128, 0, 64 },
	sf::Color{ 255, 128, 0 },
	sf::Color{ 64, 128, 64 }
};

static unsigned int side_term_font_size = 20;
static float side_term_left = 24.0f;
static float side_term_top = 9.0f;
static float side_term_lineheight = 18.0f;

static unsigned int bottom_term_font_size = 15;
static float bottom_term_left = 17.0f;
static float bottom_term_top = 7.0f;
static float bottom_term_lineheight = 12.0f;


sf::Color GetFromID(int id)
{
	return presetColours[id % 24];
}

float RandToSinRange(unsigned int input)
{
	return sinf((input % 31415) * 0.0001f);
}


void ToImColour(sf::Color &bgColor, float color[3])
{
	color[0] = float(bgColor.r) / 255.0f;
	color[1] = float(bgColor.g) / 255.0f;
	color[2] = float(bgColor.b) / 255.0f;
}

void ToSFMLColor(sf::Color &bgColor, float color[3])
{
	bgColor.r = static_cast<sf::Uint8>(color[0] * 255.f);
	bgColor.g = static_cast<sf::Uint8>(color[1] * 255.f);
	bgColor.b = static_cast<sf::Uint8>(color[2] * 255.f);
}

void LaunchProg(GameProgram& newProg) {
	progs.push_back(newProg);
	grequest.SpawnNewRequest(newProg);

	TermLine line = TermLine("", newProg.color);
	sprintf_s(line.str, "Launched #%d waiting for %d memory", newProg.prog_id, newProg.cells_requested);
	termlines.push_front(line);
};

void StartExecution(GameProgram &p)
{
	running_pid = p.prog_id;

	while (prog_todo.empty() == false)
		prog_todo.pop();

	bool reqMem = ((game_rand() % 5) == 0);
	int exeCycle = (game_rand() % 4);
	int freeMem = 0;//  (game_rand() % p.cells_in_mem) / 3;
	int readMem = 1 + (game_rand() % 3);
	
	while ((exeCycle + freeMem + readMem) > 0) {
		switch (game_rand() % 3) {
		case 0:
		{
			if (freeMem > 0) {
				prog_todo.push(ProgInstructions::FreeMem);
				freeMem--;
			}
		} break;
		case 1:
		{
			if (readMem > 0) {
				prog_todo.push(ProgInstructions::ReadMem);
				readMem--;
			}
		} break;
		case 2:
		{
			if (exeCycle > 0) {
				prog_todo.push(ProgInstructions::Execute);
				exeCycle--;
			}
		} break;
		}
	}

	if (reqMem)
		prog_todo.push(ProgInstructions::ReqMem);

	TermLine line = TermLine("", p.color);
	sprintf_s(line.str, "#%d > executing...", p.prog_id);
	termlines.push_front(line);

	prog_doing.todo = NOOF_ProgInstructions;

}

bool MemSeek(int pid, int offset, sf::Vector2i& gridPos) {

	for (int i = 0; i < ggrid.cellData.size(); i++) {
		const GameCell& c = ggrid.cellData[i];

		if ((c.prog_id == pid) && (c.offset == offset)) {
			gridPos = sf::Vector2i{ i % ggrid.numCells.x, i / ggrid.numCells.x };
			sf::Vector2f pos = ggrid.GetCellCenter(gridPos);
			pos = ggrid.getTransform().transformPoint(pos);
			garrow.dest.x = pos.x;
			garrow.dest.y = pos.y;
			garrow.dest.z = 90.0f;

			return garrow.AtDest();
		}
	}

	IM_ASSERT(false);
	return true; // To avoid lock up
}

bool MemRead(int pid, int offset, sf::Vector2i& gridPos) {

	for (int i = 0; i < ggrid.cellData.size(); i++) {
		const GameCell& c = ggrid.cellData[i];

		if ((c.prog_id == pid) && (c.offset == offset)) {
			gridPos = sf::Vector2i{ i % ggrid.numCells.x, i / ggrid.numCells.x };
			sf::Vector2f pos = ggrid.GetCellCenter(gridPos);
			pos = ggrid.getTransform().transformPoint(pos);
			garrow.dest.x = pos.x;
			garrow.dest.y = pos.y;
			garrow.dest.z = 270.0f;

			return garrow.AtDest();
		}
	}

	IM_ASSERT(false);
	return true; // To avoid lock up
}

int WinMain()
{
	sf::RenderWindow window(sf::VideoMode(1280, 720), "SFML works!");
	window.setVerticalSyncEnabled(true);
	ImGui::SFML::Init(window);

	std::string logfile = "sfml-log.txt";
	std::ofstream file(logfile, std::ios::binary);
	sf::err().rdbuf(file.rdbuf());

	window.resetGLStates(); // call it if you only draw ImGui. Otherwise not needed.
	sf::Clock deltaClock;

	g_console_font.loadFromFile("5by5.ttf");

	// Background
	sf::Color bgColor = { 100, 100, 100 };
	sf::Texture background_img;
	sf::Sprite background;
	{
		background_img.loadFromFile("background.png");
		background.setTexture(background_img);
	}

	sf::Texture side_term_tex;
	sf::Sprite side_term;
	{
		side_term_tex.loadFromFile("side_term.png");
		side_term.setTexture(side_term_tex);

		side_term.setPosition(sf::Vector2f{ 0, window.getSize().y*0.5f - side_term_tex.getSize().y*0.5f });
	}

	sf::Texture bottom_term_tex;
	sf::Sprite bottom_term;
	{
		bottom_term_tex.loadFromFile("bottom_term.png");
		bottom_term.setTexture(bottom_term_tex);

		bottom_term.setPosition(sf::Vector2f{ window.getSize().x*0.4f - bottom_term_tex.getSize().x*0.5f, float(window.getSize().y - bottom_term_tex.getSize().y) });
	}

	sf::Texture arrow_tex;
	{
		arrow_tex.loadFromFile("arrow.png");
		garrow.spr.setTexture(arrow_tex);
		garrow.spr.setOrigin(float(arrow_tex.getSize().x), arrow_tex.getSize().y * 0.5f);
	}


	char windowTitle[255] = "ImGui + SFML = <3";
	window.setTitle(windowTitle);

	//////////////////////////////////////////////////////////////////////////// Setup Game Bullshit
	game_rand = std::mt19937(std::chrono::high_resolution_clock::now().time_since_epoch().count());
	progs.clear();
	termlines.clear();
	while (prog_todo.empty() == false) prog_todo.pop();
	prog_doing.todo = NOOF_ProgInstructions;

	running_pid = -1;
	prog_id_counter = 1;
	seconds_till_prog_Spawn = 0;

	garrow.visible = false;

	ggrid = GameGrid(8, 12);
	ggrid.border_size = 2.0f;
	ggrid.cell_size.x = floorf(window.getSize().y * 0.7f / ggrid.numCells.y);
	ggrid.cell_size.y = ggrid.cell_size.x;
	ggrid.RebuildVerts();
	ggrid.setPosition(sf::Vector2f{
		300.0f,
		window.getSize().y * 0.05f
		});

	grequest = GameRequestBoard(game_rand());
	grequest.setPosition(sf::Vector2f{
		window.getSize().x - grequest.background.getLocalBounds().width - 50.0f,
		window.getSize().y * 0.5f - grequest.background.getLocalBounds().height * 0.5f
		});



	while (window.isOpen())
	{
		sf::Time delta = deltaClock.restart();

		sf::Event event;
		while (window.pollEvent(event))
		{
			ImGui::SFML::ProcessEvent(event);

			switch (event.type) {
			case sf::Event::Closed:
				window.close();
				break;

			case sf::Event::KeyReleased:
			{
				switch (event.key.code)
				{
				case sf::Keyboard::S:
					seconds_till_prog_Spawn = 0.0f;

				default:
					break;
				}

			} break;

			default:
				break;
			}

		}

		auto mouseVec = sf::Mouse::getPosition(window);
		sf::Vector2f mouseVecf = sf::Vector2f(mouseVec);

		////////////////////////////////////////////////////////////////////////// GAME LOGIC UPDATE
		{
			LogicFeedback logicfb;

			if (sf::Mouse::isButtonPressed(sf::Mouse::Left)) {

				if (g_renderFeedback.dragged == nullptr) {
					// Do selection
					if (g_renderFeedback.hovered == &ggrid) {
						ggrid.selected = g_renderFeedback.hover_cell;
					}
					else {
						ggrid.selected = sf::Vector2i(-1, -1);
					}

					if (g_renderFeedback.hovered == &grequest) {
						grequest.selected = g_renderFeedback.hover_cell;
					}
					else {
						grequest.selected = sf::Vector2i(-1, -1);
					}

					g_renderFeedback.drag_time = sf::Time::Zero;
					g_renderFeedback.dragStartPos = g_renderFeedback.cursorPos;
					g_renderFeedback.dragged = g_renderFeedback.hovered;
					g_renderFeedback.drag_cells = g_renderFeedback.hover_cell;
				}

				g_renderFeedback.drag_time += delta;
			}
			else {


				if (g_renderFeedback.dragged != nullptr) {
					if ((g_renderFeedback.hovered == &ggrid) && (g_renderFeedback.dragged == &grequest)) {
						if (GameRequest* req = grequest.GetRequest(g_renderFeedback.drag_cells.y)) {
							int dropped = ggrid.Drop(*req);
							if (dropped > 0) {
								TermLine line = TermLine("", req->color);
								sprintf_s(line.str, " #%d > %d cells allocated in mem", req->prog_id, dropped);
								termlines.push_front(line);
							}
						}
					}

					if ((g_renderFeedback.hovered != g_renderFeedback.dragged) || (g_renderFeedback.drag_time.asSeconds() > 0.5f)) {
						ggrid.selected = sf::Vector2i{ -1,-1 };
						grequest.selected = sf::Vector2i{ -1, -1 };
					}

					g_renderFeedback.dragged = nullptr;
					g_renderFeedback.drag_cells = sf::Vector2i(-1, -1);
				}
			}

			if (seconds_till_prog_Spawn <= 0) { // <---------------------------------------- SPAWN NEW PROGS
				if (progs.size() < 15)
					seconds_till_prog_Spawn = 0.5f;
				else 
					seconds_till_prog_Spawn = 15.0f + 15.0f * RandToSinRange(game_rand());

				GameProgram newProg;
				newProg.prog_id = prog_id_counter++;
				newProg.cells_in_mem = 0;
				newProg.cells_requested = 2 + game_rand() % 6;
				newProg.color = GetFromID(newProg.prog_id);

				LaunchProg(newProg);
			}
			seconds_till_prog_Spawn -= delta.asSeconds();

			// <----------------------------- Go Go Go mystical program fuckery
			if (running_pid < 0) {
				for (GameProgram& p : progs) {
					if (p.cells_requested <= 0) {
						StartExecution(p);
						break;
					}
				}
			}

			if (running_pid >= 0) {
				GameProgram* runner = nullptr;
				for (GameProgram& p : progs)
					if (p.prog_id == running_pid)
						runner = &p;

				if ((runner != nullptr) && (prog_doing.todo == NOOF_ProgInstructions)) {
					TermLine line = TermLine("", runner->color);

					if (prog_todo.empty()) {
						logicfb.wipe_progid.push_back(running_pid);
						sprintf_s(line.str, "#%d > exit!", running_pid);
						termlines.push_front(line);

						running_pid = -1;
						garrow.visible = false;
					}
					else {
						prog_doing.todo = prog_todo.front();
						prog_doing.stage = 0;
						prog_todo.pop();

						switch (prog_doing.todo) {

						case ProgInstructions::FreeMem:
						if(runner->cells_in_mem > 1)
						{
							prog_doing.a = int(game_rand() % runner->cells_in_mem);
							prog_doing.b = 1;

							sprintf_s(line.str, "#%d > freeing mem cells", running_pid);
							termlines.push_front(line);
							break;
						}
						else {
							prog_doing.todo = ProgInstructions::ReadMem;
						}// << --- Intertional Waterfall
						case ProgInstructions::ReadMem:
						{
							prog_doing.a = int(game_rand() % runner->cells_in_mem);
							prog_doing.b = 1 + int((game_rand() % runner->cells_in_mem) + (game_rand() % runner->cells_in_mem));

							sprintf_s(line.str, "#%d > reading %d mem cells", running_pid, prog_doing.a);
							termlines.push_front(line);
						} break;


						case ProgInstructions::ReqMem:
						{
							prog_doing.a = 1 + (game_rand() % 3) + (game_rand() % 3);
							runner->cells_requested += prog_doing.a;
							grequest.SpawnNewRequest(*runner);

							sprintf_s(line.str, "#%d > requesting %d mem cells", running_pid, prog_doing.a);
							termlines.push_front(line);
							prog_doing.todo = NOOF_ProgInstructions;

						} break;

						case ProgInstructions::Execute:
						{
							prog_doing.time = sf::seconds(0.5f);

							sprintf_s(line.str, "#%d > processing...", running_pid);
							termlines.push_front(line);
						} break;

						}
					}

				}

				// Execute Current Instruction
				sf::Vector2i seekPos;
				sf::Vector2i readPos;

				switch (prog_doing.todo) {
				case ProgInstructions::ReadMem:
					if (prog_doing.stage == 0) {
						if (MemSeek(running_pid, prog_doing.a, seekPos))
							prog_doing.stage = 1;
					}
					else if (prog_doing.stage == 1) {
						if (MemRead(running_pid, prog_doing.a, readPos)) {
							prog_doing.stage = 2;
							prog_doing.time = sf::seconds(0.5f);
						}
					}
					else {
						prog_doing.time -= delta;

						if (prog_doing.time.asSeconds() < 0) {
							MemRead(running_pid, prog_doing.a, seekPos); // getting Pos

							prog_doing.b--;
							prog_doing.a = (prog_doing.a + 1) % runner->cells_in_mem;

							if (prog_doing.b <= 0) {
								prog_doing.todo = NOOF_ProgInstructions;
							}
							else {
								MemRead(running_pid, prog_doing.a, readPos); // getting Pos
								if ((seekPos.y == readPos.y) && ((seekPos.x + 1) == readPos.x))
									prog_doing.stage = 1; // Back to reading
								else {
									prog_doing.stage = 0; // Back to seeking
									MemSeek(running_pid, prog_doing.a, seekPos);
								}
							}
						}
					}
					break;

				case ProgInstructions::FreeMem:
					if (prog_doing.stage == 0) {
						if (MemSeek(running_pid, prog_doing.a, seekPos)) {
							prog_doing.stage = 1;
							prog_doing.time = sf::seconds(1.0f);
						}
					}
					else if (prog_doing.stage == 1) {
						if (MemRead(running_pid, prog_doing.a, seekPos)) {
							prog_doing.stage = 2;
						}
					}
					else {
						prog_doing.time -= delta;

						if (prog_doing.time.asSeconds() < 0) {
							for (GameProgram& p : progs)
								if (p.prog_id == running_pid)
									p.cells_in_mem--;

							for (GameCell& c : ggrid.cellData)
								if (c.prog_id == running_pid)
								{
									if (c.offset == prog_doing.a) {
										c.color = sf::Color::Transparent;
										c.prog_id = -1;
										c.offset = -1;
									}
									else if (c.offset > prog_doing.a)
										c.offset--;
								}

							prog_doing.todo = NOOF_ProgInstructions;
						}
					}
					break;

				case ProgInstructions::ReqMem:
					// Do Nothing
					break;

				case ProgInstructions::Execute:
					garrow.dest = g_renderFeedback.execPos;
					if (garrow.AtDest()) {
						prog_doing.time -= delta;
						if (prog_doing.time.asSeconds() < 0) {
							prog_doing.todo = NOOF_ProgInstructions;
						}
					}
					break;
				default:
					break;
				};
			}

			grequest.UpdateRequests(logicfb);					// <<--------------- UPdates
			ggrid.Update(logicfb);


			for (int pid : logicfb.explode_progid) {
				GameProgram newProg;

				auto wipe = std::remove_if(progs.begin(), progs.end(), [&](GameProgram& p) -> bool {
					if (p.prog_id != pid)
						return false;

					newProg = p;
					newProg.prog_id = prog_id_counter++;
					newProg.cells_requested = newProg.cells_requested + newProg.cells_in_mem;
					newProg.cells_in_mem = 0;


					TermLine line = TermLine("", p.color);
					sprintf_s(line.str, "Restarting #%d due to memory halt", p.prog_id);
					termlines.push_front(line);

					return true;
				});

				if (wipe != progs.end()) {
					progs.erase(wipe);

					LaunchProg(newProg);
				}

				if (pid == g_renderFeedback.prog_hover_prev) {
					g_renderFeedback.dragged = nullptr;
					g_renderFeedback.drag_cells = sf::Vector2i(-1, -1);
				}
			}

			for (int pid : logicfb.placed_progid) {
				for (GameProgram& p : progs) {
					if (p.prog_id != pid)
						continue;

					p.cells_in_mem += p.cells_requested;
					p.cells_requested = 0;
				}

				if (pid == g_renderFeedback.prog_hover_prev) {
					g_renderFeedback.dragged = nullptr;
					g_renderFeedback.drag_cells = sf::Vector2i(-1, -1);
				}
			}
		}


		// Arrow Logic
		if (garrow.visible) {
			sf::Vector2f vel;
			vel.x = garrow.dest.x - garrow.pos.x;
			vel.y = garrow.dest.y - garrow.pos.y;

			float vlen = sqrtf(vel.x*vel.x + vel.y*vel.y);
			if (vlen > 5.0f) {
				vel.x = vel.x * 400.0f / vlen * delta.asSeconds();
				vel.y = vel.y * 400.0f / vlen * delta.asSeconds();
			}
			garrow.pos.x += vel.x;
			garrow.pos.y += vel.y;

			float angVel = garrow.dest.z - garrow.pos.z;
			if (angVel > 2.0f)
				angVel = 180.0f * delta.asSeconds();
			else if (angVel < -2.0f)
				angVel = -180.0f * delta.asSeconds();
			garrow.pos.z += angVel;
		}
		else {
			garrow.pos = garrow.dest;
		}


		//////////////////////////////////////////////////////////////////////////   IMGUI UPDATE
		{
			ImGui::SFML::Update(window, deltaClock.restart());
			ImGui::Begin("Sample window"); // begin window

										   // Background color edit
			float color[3];
			ToImColour(bgColor, color);
			if (ImGui::ColorEdit3("Background color", color)) {
				ToSFMLColor(bgColor, color);
			}

			side_term_font_size = 20;
			ImGui::DragFloat("side_term_left", &side_term_left);
			ImGui::DragFloat("side_term_top", &side_term_top);
			ImGui::DragFloat("side_term_lineheight", &side_term_lineheight);

			ImGui::DragFloat("bottom_term_left", &bottom_term_left);
			ImGui::DragFloat("bottom_term_top", &bottom_term_top);
			ImGui::DragFloat("bottom_term_lineheight", &bottom_term_lineheight);

			ImGui::End(); // end window
		}


		//////////////////////////////////////////////////////////////////////////  RENDER
		{
			g_renderFeedback.cursorPos = mouseVecf;
			g_renderFeedback.hovered = nullptr;
			if ((g_renderFeedback.dragged == false) || (g_renderFeedback.prog_hover_prev < 0))
				g_renderFeedback.prog_hover_prev = g_renderFeedback.prog_hovered;
			g_renderFeedback.prog_hovered = -1;

			window.clear(bgColor);

			window.draw(background);

			window.draw(side_term);			// <-------------- Side Term
			{
				sf::FloatRect gBounds = side_term.getGlobalBounds();

				float yPos = gBounds.top + side_term_top;
				char buffer[100];
				for (GameProgram& p : progs) {
					if (p.prog_id == running_pid) {
						sprintf_s(buffer, 100, "> #%03d", p.prog_id);
						g_renderFeedback.execPos = sf::Vector3f{
							gBounds.left + gBounds.width - 20.0f,
						yPos + side_term_font_size * 0.5f,
						180.0f
						};

						if (garrow.visible == false) {
							garrow.pos = g_renderFeedback.execPos;
							garrow.dest = garrow.pos;
							garrow.visible = true;
						}
					}
					else if (p.cells_requested > 0)
						sprintf_s(buffer, 100, "! #%03d", p.prog_id);
					else
						sprintf_s(buffer, 100, "  #%03d", p.prog_id);

					sf::Text sidetext = sf::Text(buffer, g_console_font, side_term_font_size);
					sidetext.setPosition(gBounds.left + side_term_left, yPos);
					sidetext.setFillColor(p.color);

					if (sidetext.getGlobalBounds().contains(g_renderFeedback.cursorPos)) {
						g_renderFeedback.prog_hovered = p.prog_id;
						sidetext.setFillColor(sf::Color::Black);
						sidetext.setOutlineColor(p.color);
						sidetext.setOutlineThickness(5.0f);
					}
					else if (g_renderFeedback.prog_hover_prev == p.prog_id) {
						sidetext.setFillColor(sf::Color::Black);
						sidetext.setOutlineColor(p.color);
						sidetext.setOutlineThickness(3.0f);
					}

					window.draw(sidetext);

					yPos += side_term_lineheight;
				}
			}

			window.draw(bottom_term);			// <---- Bottom Terminal
			{
				float yPos = bottom_term.getGlobalBounds().top + bottom_term_top;
				for (TermLine& line : termlines) {

					sf::Text term_text = sf::Text(line.str, g_console_font, bottom_term_font_size);
					term_text.setPosition(bottom_term.getGlobalBounds().left + bottom_term_left, yPos);
					term_text.setFillColor(line.color);

					window.draw(term_text);

					yPos += bottom_term_lineheight;
				}

				while (termlines.size() > 15) {
					termlines.pop_back();
				}
			}

			window.draw(ggrid);
			window.draw(grequest);

			if (garrow.visible) {
				garrow.spr.setRotation(garrow.pos.z);
				garrow.spr.setPosition(
					garrow.pos.x,
					garrow.pos.y
				);
				window.draw(garrow.spr);
			}


			ImGui::SFML::Render(window);
			window.display();
		}
	}


	ImGui::SFML::Shutdown();

	return 0;
}
