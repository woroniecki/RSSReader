import moment from 'moment'

export const applyValidationErrors = (formik: any, response: any): void => {
  if (response.result && response.result.validationErrors) {
    response.result.validationErrors.forEach((element: any) => {
      formik.setFieldError(element.name.toLowerCase(), element.reason)
    })
  } else if (response.validationErrors) {
    response.validationErrors.forEach((element: any) => {
      formik.setFieldError(element.name.toLowerCase(), element.reason)
    })
  } else if (response.message) {
    formik.setFieldError('global', response.message)
  }
}

export const formatDate = (date: string): string => {
  return moment(date).format('DD/MM/YYYY')
}
