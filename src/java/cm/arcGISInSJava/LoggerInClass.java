package cm.arcGISInSJava;

import org.apache.log4j.Logger;

/**
 *
 */
public class LoggerInClass {
    // 初始化一个Logger对象
    private static Logger Log = Logger.getLogger(Logger.class.getName());

    // 定义一个静态方法，可以打印自定义的某个测试用例开始执行的日志信息
    public static void startTestCase() {
        Log.info("-------------------------------------------------------------------------");
        Log.info("**************** 开始 ****************");
    }

    // 定义一个静态方法，可以打印自定义的某个测试用例结束执行的日志信息
    public static void endTestCase() {
        Log.info("****************结束 ****************");
        Log.info("-------------------------------------------------------------------------");
    }

    // 定义一个静态info方法，可以打印自定义的INFO级别的日志信息
    public static void info(String message) {
        Log.info(message);
    }

    // 定义一个静态warn方法，可以打印自定义的INFO级别的日志信息
    public static void warn(String message) {
        Log.warn(message);
    }

    // 定义一个静态error方法，可以打印自定义的INFO级别的日志信息
    public static void error(String message) {
        Log.error(message);
    }

    // 定义一个静态fatal方法，可以打印自定义的INFO级别的日志信息
    public static void fatal(String message) {
        Log.fatal(message);
    }

    // 定义一个静态debug方法，可以打印自定义的INFO级别的日志信息
    public static void debug(String message) {
        Log.debug(message);
    }
}
