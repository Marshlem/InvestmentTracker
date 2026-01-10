import api from './api'

export function getDashboardSummary(date) {
  return api
    .get('/dashboard/summary', { params: { date } })
    .then(r => r.data)
}

export function getDashboardChart(payload) {
  return api
    .post('/dashboard/chart', payload)
    .then(r => r.data)
}

export function getInvestmentTable() {
  return api
    .get('/dashboard/investment-table')
    .then(r => r.data)
}