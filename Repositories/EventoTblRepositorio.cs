using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PROJETO.Interfaces;
using PROJETO.Models;

namespace PROJETO.Repositories
{
    public class EventoTblRepositorio : IEventoRepositorio
    {
        EventShareContext context = new EventShareContext();

        public async Task<EventoTbl> BuscarPorId(int id)
        {
            return await context.EventoTbl.FindAsync(id);
        }
        
        public async Task<List<EventoTbl>> ListarEventos()
        {
            return await context.EventoTbl.Include(Ec => Ec.EventoCategoria).Include(Eesp => Eesp.EventoEspaco).Include(Es => Es.EventoStatus).Include(ecria => ecria.CriadorUsuario).Include(rev => rev.ResponsavelUsuario).ToListAsync();
        }

        public async Task<List<int>> BuscarEspacoPorData(DateTime data){
            List<EventoTbl> listaDeEventos = await context.EventoTbl.Where(e => e.EventoData != data).ToListAsync();
            List<int> IdsDeEspacosVazios = new List<int>();

         foreach (var item in listaDeEventos){
            IdsDeEspacosVazios.Add(item.EventoEspacoId);
         }
            
            return IdsDeEspacosVazios;
        }

        public async Task<EventoTbl> BuscarPorNome(string nomeEvento){
            string nomeDoEvento = nomeEvento.ToLower();
            var retorno = await context.EventoTbl.Include(Ec => Ec.EventoCategoria).Include(Eesp => Eesp.EventoEspaco).Include(Es => Es.EventoStatus).Include(ecria => ecria.CriadorUsuario).Include(rev => rev.ResponsavelUsuario).FirstOrDefaultAsync(x => x.EventoNome == nomeEvento);
            return retorno;
        }
    
        public async Task<List<EventoTbl>> BuscarPalavraChave(string nome){
            List<EventoTbl> listaE = await context.EventoTbl.Where(e => e.EventoNome.Contains(nome)).Include(Es => Es.EventoStatus).Include(Ec => Ec.EventoCategoria).Include(Eesp => Eesp.EventoEspaco).Include(rev => rev.ResponsavelUsuario).ToListAsync();
            return listaE;
        }

        public async Task<List<EventoTbl>> BuscarPorCategoria(int id){
            List<EventoTbl> listaE = await context.EventoTbl.Where(ev => ev.EventoCategoria.CategoriaId == id).Include(Ec => Ec.EventoCategoria).Include(Eesp => Eesp.EventoEspaco).Include(Es => Es.EventoStatus).Include(ecria => ecria.CriadorUsuario).Include(rev => rev.ResponsavelUsuario).ToListAsync();
            return listaE;
        }

        public async Task<List<EventoTbl>> BuscarPorData(DateTime data){
            List<EventoTbl> listaE = await context.EventoTbl.Where(ev => ev.EventoData == data).ToListAsync();
            return listaE;
        }
        
        public async Task<List<EventoTbl>> BuscarPorStatus(string status){
            List<EventoTbl> listaE = await context.EventoTbl.Where(ev => ev.EventoStatus.EventoStatusNome.Contains(status)).Include(Ec => Ec.EventoCategoria).Include(Eesp => Eesp.EventoEspaco).Include(Es => Es.EventoStatus).Include(ecria => ecria.CriadorUsuario).Include(rev => rev.ResponsavelUsuario).ToListAsync();
            return listaE;
        }
        public async Task<EventoTbl> Post(EventoTbl evento){
        
            EventoTbl eventoCadastrado = evento;
            
            await context.EventoTbl.AddAsync(eventoCadastrado);
            await context.SaveChangesAsync();
            return eventoCadastrado;
        }

        public async Task<EventoTbl> Put(EventoTbl evento){
            EventoTbl eventoEncontrado = await context.EventoTbl.FindAsync(evento.EventoId);
            
            // eventoEncontrado.EventoNome = evento.EventoNome.ToLower();
            // eventoEncontrado.EventoHorarioComeco = evento.EventoHorarioComeco;
            // eventoEncontrado.EventoHorarioFim = evento.EventoHorarioFim;
            // eventoEncontrado.EventoData = evento.EventoData;
            // eventoEncontrado.EventoDescricao = evento.EventoDescricao;
            // eventoEncontrado.EventoEspacoId = evento.EventoEspacoId;
            // eventoEncontrado.ResponsavelUsuarioId = evento.ResponsavelUsuarioId;
            // eventoEncontrado.EventoCategoriaId = evento.EventoCategoriaId;
                        
            context.Entry(eventoEncontrado).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return eventoEncontrado;
        }
        public async Task<EventoTbl> DeletarEvento(int id){
            EventoTbl eventoRetornado = await context.EventoTbl.FindAsync(id);

            context.EventoTbl.Remove(eventoRetornado);
            await context.SaveChangesAsync();
            return eventoRetornado;
        }

        public Task<EventoTbl> UploadImagem(string imagem)
        {
            throw new NotImplementedException();
        }
    }
}
