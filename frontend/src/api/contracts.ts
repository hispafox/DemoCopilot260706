export enum PrioridadTarea {
  Baja = 1,
  Normal = 2,
  Alta = 3,
  Urgente = 4
}

export enum TipoRecurrencia {
  Diaria = 1,
  Semanal = 2,
  Mensual = 3
}

export interface TareaDto {
  id: number;
  titulo: string;
  estaCompletada: boolean;
  fechaCreacion: string;
  fechaVencimiento: string | null;
  notas: string | null;
  prioridad: PrioridadTarea;
  esRepetitiva: boolean;
  tipoRecurrencia: TipoRecurrencia | null;
  proximaRecurrencia: string | null;
  plantillaTareaId: number | null;
  categoriaId: number | null;
  usuarioId: number | null;
  usuarioNombre: string | null;
  tipoTareaId: number;
  tipoTareaNombre: string;
}

export interface CrearActualizarTareaRequest {
  titulo: string;
  estaCompletada: boolean;
  fechaVencimiento: string | null;
  notas: string | null;
  prioridad: PrioridadTarea;
  esRepetitiva: boolean;
  tipoRecurrencia: TipoRecurrencia | null;
  proximaRecurrencia: string | null;
  plantillaTareaId: number | null;
  categoriaId: number | null;
  usuarioId: number | null;
  tipoTareaId: number;
}

export interface TipoTareaDto {
  id: number;
  nombre: string;
  descripcion: string | null;
  estaActivo: boolean;
}

export interface CrearActualizarTipoTareaRequest {
  nombre: string;
  descripcion: string | null;
  estaActivo: boolean;
}

export interface PlantillaTareaDto {
  id: number;
  titulo: string;
  notas: string | null;
  esRepetitiva: boolean;
  tipoRecurrencia: TipoRecurrencia | null;
  categoriaId: number | null;
  estaActiva: boolean;
}

export interface CrearActualizarPlantillaTareaRequest {
  titulo: string;
  notas: string | null;
  esRepetitiva: boolean;
  tipoRecurrencia: TipoRecurrencia | null;
  categoriaId: number | null;
  estaActiva: boolean;
}

export interface UsuarioDto {
  id: number;
  nombre: string;
  email: string | null;
  departamentoId: number;
  departamentoNombre: string;
  sedeId: number;
  sedeNombre: string;
  poblacionId: number;
  poblacionNombre: string;
  poblacionCodigoIsoPais: string;
}

export interface CrearActualizarUsuarioRequest {
  nombre: string;
  email: string | null;
  departamentoId: number;
  sedeId: number;
  poblacionId: number;
}

export interface DepartamentoDto {
  id: number;
  nombre: string;
}

export interface CrearActualizarDepartamentoRequest {
  nombre: string;
}

export interface SedeDto {
  id: number;
  nombre: string;
}

export interface CrearActualizarSedeRequest {
  nombre: string;
}

export interface PoblacionDto {
  id: number;
  nombre: string;
  codigoIsoPais: string;
}

export interface CrearActualizarPoblacionRequest {
  nombre: string;
  codigoIsoPais: string;
}