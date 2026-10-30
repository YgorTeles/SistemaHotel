using SistemaHotelaria.Controllers;
using SistemaHotelaria.Repositories;
using SistemaHotelaria.Services;

HospedeRepository hospedeRepository = new HospedeRepository();
HospedeService hospedeService = new HospedeService(hospedeRepository);
HospedeController hospedeController = new HospedeController(hospedeService);

hospedeController.Cadastrar();
hospedeController.Cadastrar();