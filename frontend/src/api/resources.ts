import { apiRequest } from './client';
import type {
  CrearActualizarDepartamentoRequest,
  CrearActualizarPoblacionRequest,
  CrearActualizarPlantillaTareaRequest,
  CrearActualizarSedeRequest,
  CrearActualizarTareaRequest,
  CrearActualizarTipoTareaRequest,
  CrearActualizarUsuarioRequest,
  DepartamentoDto,
  PoblacionDto,
  PlantillaTareaDto,
  SedeDto,
  TareaDto,
  TipoTareaDto,
  UsuarioDto
} from './contracts';

export function getTareas() {
  return apiRequest<TareaDto[]>('/tareas');
}

export function createTarea(request: CrearActualizarTareaRequest) {
  return apiRequest<TareaDto>('/tareas', {
    method: 'POST',
    body: JSON.stringify(request)
  });
}

export function updateTarea(id: number, request: CrearActualizarTareaRequest) {
  return apiRequest<TareaDto>(`/tareas/${id}`, {
    method: 'PUT',
    body: JSON.stringify(request)
  });
}

export function deleteTarea(id: number) {
  return apiRequest<void>(`/tareas/${id}`, {
    method: 'DELETE'
  });
}

export function completeTarea(id: number) {
  return apiRequest<TareaDto>(`/tareas/${id}/completar`, {
    method: 'POST'
  });
}

export function createTaskFromTemplate(plantillaId: number) {
  return apiRequest<TareaDto>(`/tareas/desde-plantilla/${plantillaId}`, {
    method: 'POST'
  });
}

export function getPlantillas() {
  return apiRequest<PlantillaTareaDto[]>('/plantillas');
}

export function createPlantilla(request: CrearActualizarPlantillaTareaRequest) {
  return apiRequest<PlantillaTareaDto>('/plantillas', {
    method: 'POST',
    body: JSON.stringify(request)
  });
}

export function updatePlantilla(id: number, request: CrearActualizarPlantillaTareaRequest) {
  return apiRequest<PlantillaTareaDto>(`/plantillas/${id}`, {
    method: 'PUT',
    body: JSON.stringify(request)
  });
}

export function deletePlantilla(id: number) {
  return apiRequest<void>(`/plantillas/${id}`, {
    method: 'DELETE'
  });
}

export function getUsuarios() {
  return apiRequest<UsuarioDto[]>('/usuarios');
}

export function createUsuario(request: CrearActualizarUsuarioRequest) {
  return apiRequest<UsuarioDto>('/usuarios', {
    method: 'POST',
    body: JSON.stringify(request)
  });
}

export function updateUsuario(id: number, request: CrearActualizarUsuarioRequest) {
  return apiRequest<UsuarioDto>(`/usuarios/${id}`, {
    method: 'PUT',
    body: JSON.stringify(request)
  });
}

export function deleteUsuario(id: number) {
  return apiRequest<void>(`/usuarios/${id}`, {
    method: 'DELETE'
  });
}

export function getDepartamentos() {
  return apiRequest<DepartamentoDto[]>('/departamentos');
}

export function createDepartamento(request: CrearActualizarDepartamentoRequest) {
  return apiRequest<DepartamentoDto>('/departamentos', {
    method: 'POST',
    body: JSON.stringify(request)
  });
}

export function updateDepartamento(id: number, request: CrearActualizarDepartamentoRequest) {
  return apiRequest<DepartamentoDto>(`/departamentos/${id}`, {
    method: 'PUT',
    body: JSON.stringify(request)
  });
}

export function deleteDepartamento(id: number) {
  return apiRequest<void>(`/departamentos/${id}`, {
    method: 'DELETE'
  });
}

export function getSedes() {
  return apiRequest<SedeDto[]>('/sedes');
}

export function createSede(request: CrearActualizarSedeRequest) {
  return apiRequest<SedeDto>('/sedes', {
    method: 'POST',
    body: JSON.stringify(request)
  });
}

export function updateSede(id: number, request: CrearActualizarSedeRequest) {
  return apiRequest<SedeDto>(`/sedes/${id}`, {
    method: 'PUT',
    body: JSON.stringify(request)
  });
}

export function deleteSede(id: number) {
  return apiRequest<void>(`/sedes/${id}`, {
    method: 'DELETE'
  });
}

export function getPoblaciones() {
  return apiRequest<PoblacionDto[]>('/poblaciones');
}

export function createPoblacion(request: CrearActualizarPoblacionRequest) {
  return apiRequest<PoblacionDto>('/poblaciones', {
    method: 'POST',
    body: JSON.stringify(request)
  });
}

export function updatePoblacion(id: number, request: CrearActualizarPoblacionRequest) {
  return apiRequest<PoblacionDto>(`/poblaciones/${id}`, {
    method: 'PUT',
    body: JSON.stringify(request)
  });
}

export function deletePoblacion(id: number) {
  return apiRequest<void>(`/poblaciones/${id}`, {
    method: 'DELETE'
  });
}

export function getTiposTarea() {
  return apiRequest<TipoTareaDto[]>('/tipos-tarea');
}

export function createTipoTarea(request: CrearActualizarTipoTareaRequest) {
  return apiRequest<TipoTareaDto>('/tipos-tarea', {
    method: 'POST',
    body: JSON.stringify(request)
  });
}

export function updateTipoTarea(id: number, request: CrearActualizarTipoTareaRequest) {
  return apiRequest<TipoTareaDto>(`/tipos-tarea/${id}`, {
    method: 'PUT',
    body: JSON.stringify(request)
  });
}

export function deleteTipoTarea(id: number) {
  return apiRequest<void>(`/tipos-tarea/${id}`, {
    method: 'DELETE'
  });
}