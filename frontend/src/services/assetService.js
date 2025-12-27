import api from './api'

export function getAssetsSummary(date) {
  return api
    .get(`/assets/summary?date=${date}`)
    .then(r => r.data)
}

export function getAssetsLookup() {
  return api
    .get('/assets/lookup')
    .then(r => r.data)
}

export async function createAsset(asset) {
  return (await api.post('/assets', {
    name: asset.name,
    categoryId: asset.categoryId,  
    status: asset.status,
    valueCurrency: asset.valueCurrency,   
    dividendCurrency: asset.dividendCurrency 
  })).data
}

export async function updateAsset(id, updates) {
  return (await api.put(`/assets/${id}`, {
    ...updates,
    categoryId: updates.categoryId,
    valueCurrency: updates.valueCurrency   
  })).data
}

export async function deleteAsset(id) {
  await api.delete(`/assets/${id}`)
}
