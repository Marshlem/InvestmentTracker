import api from './api'

export async function getTransactionsForEdit(date) {
  return (await api.get(`/transactions/edit?date=${date}`)).data
}

export async function bulkUpsertTransactions(date, items) {
  return (await api.post('/transactions/bulk-upsert', { date, items })).data
}

export async function getTransactionHistory(payload) {
  return (await api.post('/transactions/history', payload)).data
}

export async function getAssetsLookup() {
  return (await api.get('/transactions/lookup')).data
}
