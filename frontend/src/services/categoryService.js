import api from './api'

export const getCategories = () =>
  api.get('/categories').then(r => r.data)

export const createCategory = name =>
  api.post('/categories', { name }).then(r => r.data)

export const renameCategory = (id, payload) =>
  api.put(`/categories/${id}`, payload)

export const deleteCategory = id =>
  api.delete(`/categories/${id}`)
