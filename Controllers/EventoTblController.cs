using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventShareBackend_master.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PROJETO.Models;
using PROJETO.Repositories;
using tst.Repositorio;

namespace PROJETO.Controllers {
    [ApiController]
    [Route ("api/[controller]")]
    [Produces ("application/json")]
    public class EventoTblController : ControllerBase {
        EventoTblRepositorio repositorio = new EventoTblRepositorio ();

        UploadRepositorio upload = new UploadRepositorio();

        /// <summary>
        /// Método para listar todos os eventos existentes
        /// </summary>
        /// <returns>Retorna lista de eventos</returns>
        [EnableCors]
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<EventoTbl>>> Get () {
            List<EventoTbl> listaE = await repositorio.ListarEventos ();

            foreach (var evento in listaE) {
                evento.EventoCategoria.EventoTbl = null;
                evento.EventoEspaco.EventoTbl = null;
                evento.EventoStatus.EventoTbl = null;
                evento.CriadorUsuario.EventoTblCriadorUsuario = null;
                evento.ResponsavelUsuario.EventoTblResponsavelUsuario = null;
            }

            return listaE;
        }

        /// <summary>
        /// Método para buscar evento pelo nome
        /// </summary>
        /// <returns>Retorna o evento</returns>
        /// <param name="nomeEvento"></param>
        [EnableCors]
        [AllowAnonymous]
        [HttpGet ("{nomeEvento}")]
        public async Task<ActionResult<EventoTbl>> Get (string nomeEvento) {
            EventoTbl eventoRetornado = await repositorio.BuscarPorNome (nomeEvento);

            if (eventoRetornado == null) {
                return NotFound ();
            }

            return eventoRetornado;
        }

        /// <summary>
        /// Método para listar eventos por uma palavra chave
        /// </summary>
        /// <returns>Retorna a lista de eventos</returns>
        /// <param name="nome"></param>
        [EnableCors]
        [AllowAnonymous]
        [HttpGet ("busca/{nome}")]
        public async Task<ActionResult<List<EventoTbl>>> GetKey (string nome) {
            List<EventoTbl> listaRetornada = await repositorio.BuscarPalavraChave (nome);
            return listaRetornada;
        }

        /// <summary>
        /// Método para filtrar os eventos de acordo com sua categoria
        /// </summary>
        /// <returns>Retorna a lista de eventos</returns>
        /// <param name="id"></param>
        [EnableCors]
        [AllowAnonymous]
        [HttpGet ("categoria/{id}")]
        public async Task<List<EventoTbl>> BuscarPorCategoria(int id) {
            List<EventoTbl> listaRetornada = await repositorio.BuscarPorCategoria(id);

             foreach (var evento in listaRetornada) {
                evento.EventoCategoria.EventoTbl = null;
                evento.EventoEspaco.EventoTbl = null;
                evento.EventoStatus.EventoTbl = null;
                evento.CriadorUsuario.EventoTblCriadorUsuario = null;
                evento.ResponsavelUsuario.EventoTblResponsavelUsuario = null;
            }

            return listaRetornada;
        }

        /// <summary>
        /// Método para listar os espaços vazios de acordo com a data
        /// </summary>
        /// <returns>Retorna uma lista de espaços livres na data digitada como parâmetro</returns>
        /// <param name="data"></param>
        [EnableCors]
        // [Authorize]
        [AllowAnonymous]
        [HttpGet("evento/espaco/{data}")]
        public async Task<ActionResult<List<EventoEspacoTbl>>> VerificaEspaco(DateTime data){
            EspacoRepositorio repositorioEspaco = new EspacoRepositorio();
            List<int> ListaIdsDeEspacosVazios =  await repositorio.BuscarEspacoPorData(data);
            List<EventoEspacoTbl> ListaDeEspacos = new List<EventoEspacoTbl>();

            foreach (var id in ListaIdsDeEspacosVazios){
                EventoEspacoTbl espacoAtual = await repositorioEspaco.Get(id);
                ListaDeEspacos.Add(espacoAtual);
            }

            return ListaDeEspacos;
        }



        /// <summary>
        /// Método para listar os eventos de acordo com a data 
        /// </summary>
        /// <returns>Retorna a lista de eventos com a data encontrada</returns>
        /// <param name="data"></param>
        [EnableCors]
        [AllowAnonymous]
        [HttpGet("data/{data}")]
        public async Task<ActionResult<List<EventoTbl>>> BuscarPorData(DateTime data){
            List<EventoTbl> listaRetornada = await repositorio.BuscarPorData(data);

            return listaRetornada;
        }

        /// <summary>
        /// Método para filtrar eventos pelo status, restrito somente ao administrador
        /// </summary>
        /// <returns>Retorna a lista de eventos</returns>
        /// <param name="status"></param>
        [EnableCors]
        // [Authorize(Roles = "Administrador")]
        [HttpGet ("status/(status)")]
        public async Task<List<EventoTbl>> GetStatus (string status){
            List<EventoTbl> listaRetornada = await repositorio.BuscarPorStatus(status);
            return listaRetornada;
        }

        /// <summary>
        /// Método para cadastrar um novo evento
        /// </summary>
        /// <returns>Retorna o evento criado</returns>
        /// <param name="evento"></param>
        [EnableCors]
        // [Authorize]
        [HttpPost]
        public async Task<ActionResult<EventoTbl>> Post([FromForm]EventoTbl evento) {
            try {
                // var arquivo = Request.Form.Files[0];
                // evento.EventoImagem = upload.Upload(arquivo, "images");
                evento.EventoNome = Request.Form["EventoNome"];
                evento.EventoData = DateTime.Parse(Request.Form["EventoData"]);
                evento.EventoHorarioComeco = Request.Form["EventoHorarioComeco"];
                evento.EventoHorarioFim = Request.Form["EventoHorarioFim"];
                evento.EventoDescricao = Request.Form["EventoDescricao"];
                evento.EventoCategoriaId = int.Parse(Request.Form["EventoCategoriaId"]);
                evento.EventoEspacoId = int.Parse(Request.Form["EventoEspacoId"]);
                evento.EventoStatusId = int.Parse(Request.Form["EventoStatusId"]);
                evento.CriadorUsuarioId = int.Parse(Request.Form["CriadorUsuarioId"]);
                evento.ResponsavelUsuarioId = int.Parse(Request.Form["ResponsavelUsuarioId"]);

                return await repositorio.Post(evento);

            } catch (System.ArgumentException){
                throw;
                // return BadRequest("Evento não cadastrado devido à campos inválidos.");
            }
        }

        /// <summary>
        /// Método para atualizar informaçoes de um evento previamente cadastrado
        /// </summary>
        /// <returns>Retorna o evento relacionado ao ID</returns>
        /// <param name="evento"></param>
        [EnableCors]
        // [Authorize]
        [HttpPut]
        public async Task<ActionResult<EventoTbl>> Put([FromForm]EventoTbl evento) {
            try {

                // var arquivo = Request.Form.Files[0];
                // evento.EventoImagem = upload.Upload(arquivo, "images");
                evento.EventoId = int.Parse(Request.Form["EventoId"]);
                evento.EventoNome = Request.Form["EventoNome"];
                evento.EventoData = DateTime.Parse(Request.Form["EventoData"]);
                evento.EventoHorarioComeco = Request.Form["EventoHorarioComeco"];
                evento.EventoHorarioFim = Request.Form["EventoHorarioFim"];
                evento.EventoDescricao = Request.Form["EventoDescricao"];
                evento.EventoCategoriaId = int.Parse(Request.Form["EventoCategoriaId"]);
                evento.EventoEspacoId = int.Parse(Request.Form["EventoEspacoId"]);

                return await repositorio.Put(evento);

            } 
            catch (System.Exception) {
                var eventoRetornado = await repositorio.BuscarPorId(evento.EventoId);
                if (eventoRetornado == null) {
                    return NotFound("Evento não encontrado.");
                } else {
                    return BadRequest("Impossível cadastrar devido à campos inválidos.");
                }
            } 
        }

        /// <summary>
        /// Método para deletar um evento existente
        /// </summary>
        /// <returns>Retorna o evento deletado</returns>
        /// <param name="id"></param>
        [EnableCors]
        // [Authorize]
        [HttpDelete ("{id}")]
        public async Task<ActionResult<EventoTbl>> Delete(int id) {
            try {
                return await repositorio.DeletarEvento(id);
            } 
            catch (System.Exception){
                var eventoRetornado = await repositorio.BuscarPorId (id);
                if (eventoRetornado == null) {
                    return NotFound("Evento não encontrado.");
                } else {
                    throw;
                }
            }
        }

    }
}