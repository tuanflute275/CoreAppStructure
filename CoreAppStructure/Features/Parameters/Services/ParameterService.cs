namespace CoreAppStructure.Features.Parameters.Services
{
    public class ParameterService : IParameterService
    {
        private readonly IParameterRepository _parameterRepository;
        private readonly ILogger<CategoryService> _logger;

        public ParameterService(IParameterRepository parameterRepository, ILogger<CategoryService> logger)
        {
            _parameterRepository = parameterRepository;
            _logger = logger;
        }

        public async Task<ResponseObject> FindAllAsync(string? name, string? sort, int page = 1)
        {
            try
            {
                var parameters = await _parameterRepository.FindAllAsync(name, sort);

                if (parameters.Count > 0)
                {
                    int totalRecords = parameters.Count();
                    int limit        = 10;
                    page             = page <= 1 ? 1 : page;
                    var pageData     = parameters.ToPagedList(page, limit);

                    int totalPages = (int)Math.Ceiling((double)totalRecords / limit);

                    var response = new
                    {
                        TotalRecords = totalRecords,
                        TotalPages   = totalPages,
                        Data         = pageData
                    };

                    LogHelper.LogInformation(_logger, "GET", "/api/parameter", null, response);
                    return new ResponseObject(200, "Query data successfully", response);
                }
                LogHelper.LogInformation(_logger, "GET", "/api/parameter", null, parameters);
                return new ResponseObject(200, "Query data successfully", parameters);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "GET", $"/api/category");
                return new ResponseObject(500, "Internal server error. Please try again later.", ex.Message);
            }
        }

        public async Task<ResponseObject> FindByIdAsync(int id)
        {
            try
            {
                var parameter = await _parameterRepository.FindByIdAsync(id);
                if (parameter == null)
                {
                    return new ResponseObject(404, $"Cannot find data with id {id}", null);
                }
                LogHelper.LogInformation(_logger, "GET", "/api/parameter/{id}", id, parameter);
                return new ResponseObject(200, "Query data successfully", parameter);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "GET", $"/api/parameter/{id}", id);
                return new ResponseObject(500, "Internal server error. Please try again later.", ex.Message);
            }
        }

        public async Task<ResponseObject> SaveAsync(ParameterViewModel model)
        {
            try
            {
                var existingPara = await _parameterRepository.FindByNameAsync(model.ParaName);
                if (existingPara != null)
                {
                    return new ResponseObject(400, "Parameter name already taken");
                }

                var parameter = new Parameter
                {
                    SystemId = model.SystemId,
                    ParaName = model.ParaName,
                    ParaScope = model.ParaScope,
                    ParaType = model.ParaType,
                    ParaDesc = model.ParaDesc,
                    ParaLobValue = model.ParaLobValue,
                    ParaShortValue = model.ParaShortValue,
                    AdminAccessibleFlag = model.AdminAccessibleFlag,
                    UserAccessibleFlag = model.UserAccessibleFlag,
                    CreateBy = "Admin",
                    CreateDatetime = DateTime.Now,
                };

                await _parameterRepository.AddAsync(parameter);
                LogHelper.LogInformation(_logger, "POST", "/api/parameter", model, parameter);
                return new ResponseObject(200, "Insert data successfully", parameter);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "POST", "/api/parameter", model);
                return new ResponseObject(500, "Internal server error. Please try again later.", ex.Message);
            }
        }

        public async Task<ResponseObject> UpdateAsync(int id, ParameterViewModel model)
        {
            try
            {
                var parameter = await _parameterRepository.FindByIdAsync(id);
                if (parameter == null)
                {
                    return new ResponseObject(404, $"Cannot find data with id {id}", null);
                }
               
                parameter.SystemId = model.SystemId;
                parameter.ParaName = model.ParaName;
                parameter.ParaScope = model.ParaScope;
                parameter.ParaType = model.ParaType;
                parameter.ParaDesc = model.ParaDesc;
                parameter.ParaLobValue = model.ParaLobValue;
                parameter.ParaShortValue = model.ParaShortValue;
                parameter.AdminAccessibleFlag = model.AdminAccessibleFlag;
                parameter.UserAccessibleFlag = model.UserAccessibleFlag;
                parameter.UpdateBy = "Admin";
                parameter.UpdateDatetime = DateTime.Now;

                await _parameterRepository.UpdateAsync(parameter);
                LogHelper.LogInformation(_logger, "PUT", $"/api/parameter/{id}", model, parameter);
                return new ResponseObject(200, "Update data successfully", parameter);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "PUT", $"/api/parameter/{id}", model);
                return new ResponseObject(500, "Internal server error. Please try again later.", ex.Message);
            }
        }

        public async Task<ResponseObject> DeleteAsync(int id)
        {
            try
            {
                var parameter = await _parameterRepository.FindByIdAsync(id);
                if (parameter == null)
                {
                    return new ResponseObject(404, $"Cannot find data with id {id}", null);
                }
                await _parameterRepository.DeleteAsync(parameter);
                LogHelper.LogInformation(_logger, "DELETE", $"/api/parameter/{id}", id, "Deleted successfully");
                return new ResponseObject(200, "Delete data successfully", null);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "DELETE", $"/api/parameter/{id}", id);
                return new ResponseObject(500, "Internal server error. Please try again later.", ex.Message);
            }
        }
    }
}
